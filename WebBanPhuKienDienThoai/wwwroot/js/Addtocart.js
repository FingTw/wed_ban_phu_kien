// ~/js/shopping-cart.js
$(document).ready(function () {
    function getAntiForgeryToken() {
        return $('input[name="__RequestVerificationToken"]').val();
    }

    function sendAjaxRequest(url, data, successCallback, errorCallback) {
        $.ajax({
            url: url,
            type: 'POST',
            data: $.extend(data, { __RequestVerificationToken: getAntiForgeryToken() }),
            success: function (response) {
                if (typeof response.itemCount !== 'undefined') {
                    $('#cart-count').text(response.itemCount);
                }
                if (response.success) {
                    successCallback(response);
                } else {
                    if (errorCallback) errorCallback(response);
                }
            },
            error: function (xhr) {
                console.error("AJAX Error:", xhr.responseText);
                if (xhr.status === 401) {
                    window.location.href = '/Account/Login';
                }
            }
        });
    }

    window.addToCart = function (productId, quantity) {
        sendAjaxRequest(
            '/ShoppingCart/AddToCart',
            { productId: productId, quantity: quantity },
            function (response) {
                alert(response.message); // Hiển thị thông báo thành công bằng alert
                updateTotalPrice();
            },
            function (response) {
                alert(response.message); // Hiển thị thông báo lỗi bằng alert
            }
        );
    };

    window.updateCart = function (productId, quantity) {
        sendAjaxRequest(
            '/ShoppingCart/UpdateCart',
            { productId: productId, quantity: quantity },
            function (response) {
                updateCartRow(productId, response);
            },
            function (response) {
                updateCartRow(productId, response); // Vẫn cập nhật giao diện với số lượng đã điều chỉnh
            }
        );
    };

    window.removeFromCart = function (productId) {
        sendAjaxRequest(
            '/ShoppingCart/RemoveFromCart',
            { productId: productId },
            function (response) {
                $(`tr[data-productid="${productId}"]`).remove();
                updateTotalPrice();
            }
        );
    };

    window.clearCart = function () {
        sendAjaxRequest(
            '/ShoppingCart/ClearCart',
            {},
            function (response) {
                $('tbody').empty();
                updateTotalPrice();
            }
        );
    };

    window.applyDiscount = function (discountCode) {
        sendAjaxRequest(
            '/ShoppingCart/ApplyDiscount',
            { discountCode: discountCode },
            function (response) {
                $('#discount-message').html(`<div class="alert alert-success">${response.message}</div>`);
                updateTotalPrice();
            },
            function (xhr) {
                $('#discount-message').html(`<div class="alert alert-danger">Đã xảy ra lỗi: ${xhr.responseText}</div>`);
            }
        );
    };

    function updateTotalPrice() {
        $.ajax({
            url: '/ShoppingCart/GetCartSummary',
            type: 'GET',
            success: function (response) {
                $('#total-price').text(response.totalPrice.toLocaleString('vi-VN') + ' VNĐ');
                if (response.discount > 0) {
                    $('#discount-amount').text(response.discount.toLocaleString('vi-VN') + ' VNĐ');
                    $('#final-price').text((response.totalPrice - response.discount).toLocaleString('vi-VN') + ' VNĐ');
                } else {
                    $('#discount-amount').text('0 VNĐ');
                    $('#final-price').text(response.totalPrice.toLocaleString('vi-VN') + ' VNĐ');
                }
            }
        });
    }

    function updateCartRow(productId, response) {
        var row = $(`tr[data-productid="${productId}"]`);
        var messageDiv = row.find('.quantity-message');

        if (!isNaN(response.adjustedQuantity) && response.adjustedQuantity > 0) {
            row.find('.quantity-input').val(response.adjustedQuantity);
        }

        row.find('.item-total').text(response.itemTotal.toLocaleString('vi-VN') + ' VNĐ');

        if (!response.success) {
            alert(response.message);
        }

        updateTotalPrice();
    }


    $('.add-to-cart-btn-global').on('click', function (e) {
        e.preventDefault();
        var productId = $(this).data('product-id');
        addToCart(productId, 1);
    });

    $('.quantity-increase').on('click', function () {
        var input = $(this).siblings('.quantity-input');
        var quantity = parseInt(input.val()) + 1;
        input.val(quantity).trigger('change');
    });

    $('.quantity-decrease').on('click', function () {
        var input = $(this).siblings('.quantity-input');
        var quantity = parseInt(input.val()) - 1;
        if (quantity > 0) input.val(quantity).trigger('change');
    });

    $('.quantity-input').on('change', debounce(function () {
        var productId = $(this).data('productid');
        var quantity = $(this).val();
        updateCart(productId, quantity);
    }, 500));

    $('.remove-item').on('click', function (e) {
        e.preventDefault();
        var productId = $(this).data('productid');
        removeFromCart(productId);
    });

    $('#clear-cart').on('click', function (e) {
        e.preventDefault();
        clearCart();
    });

    $('#apply-discount').on('click', function (e) {
        e.preventDefault();
        var discountCode = $('#discountCode').val();
        applyDiscount(discountCode);
    });

    function debounce(func, wait) {
        let timeout;
        return function () {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this, arguments), wait);
        };
    }
});