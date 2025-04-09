// ~/js/shopping-cart.js

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

$(document).ready(function () {
    console.log("shopping-cart.js loaded"); // Debug

    window.addToCart = function (productId, quantity) {
        sendAjaxRequest(
            '/ShoppingCart/AddToCart',
            { productId: productId, quantity: quantity },
            function (response) {
                alert(response.message);
                updateTotalPrice();
            },
            function (response) {
                alert(response.message);
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
                updateCartRow(productId, response);
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
                console.log("GetCartSummary response:", response); // Debug
                $('#total-price').text(response.totalPrice.toLocaleString('vi-VN') + ' VNĐ');
                if (response.discount > 0) {
                    $('#discount-amount').text(response.discount.toLocaleString('vi-VN') + ' VNĐ');
                    $('#final-price').text((response.totalPrice - response.discount).toLocaleString('vi-VN') + ' VNĐ');
                } else {
                    $('#discount-amount').text('0 VNĐ');
                    $('#final-price').text(response.totalPrice.toLocaleString('vi-VN') + ' VNĐ');
                }
            },
            error: function (xhr) {
                console.error("GetCartSummary error:", xhr.responseText);
            }
        });
    }

    function updateCartRow(productId, response) {
        var row = $(`tr[data-productid="${productId}"]`);
        if (response.success) {
            row.find('.quantity-input').val(response.quantity || quantity); // Cần server trả về quantity nếu có
            row.find('.item-total').text((response.itemTotal || 0).toLocaleString('vi-VN') + ' VNĐ');
        } else {
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
        console.log("Increase clicked");
        var input = $(this).siblings('.quantity-input');
        var quantity = parseInt(input.val()) + 1;
        input.val(quantity).trigger('change');
    });

    $('.quantity-decrease').on('click', function () {
        console.log("Decrease clicked");
        var input = $(this).siblings('.quantity-input');
        var quantity = parseInt(input.val()) - 1;
        if (quantity > 0) input.val(quantity).trigger('change');
    });

    $('.quantity-input').on('change', debounce(function () {
        console.log("Quantity changed");
        var productId = $(this).data('productid');
        var quantity = parseInt($(this).val());
        var maxStock = parseInt($(this).data('stock'));
        if (quantity > maxStock) {
            alert('Số lượng vượt quá tồn kho!');
            $(this).val(maxStock);
            quantity = maxStock;
        } else if (quantity < 1) {
            $(this).val(1);
            quantity = 1;
        }
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