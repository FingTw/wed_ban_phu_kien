
let page = 1;
let isLoading = false;
let hasMoreProducts = true;
let observer = null;

function loadProducts() {
    if (isLoading || !hasMoreProducts) return;
    isLoading = true;

    $.ajax({
        url: '@Url.Action("LoadMoreProduct", "Product")',
        type: 'GET',
        data: { page: page },
        success: function (data) {
            if (data.trim() === '') {
                hasMoreProducts = false;
                if (observer) observer.disconnect();
            } else {
                $('#product-container').append(data);
                page++;
            }
            isLoading = false;
        },
        error: function () {
            isLoading = false;
        }
    });
}

$(document).ready(function () {
    loadProducts(); // Tải trang đầu tiên

    let trigger = document.getElementById('load-trigger');
    if (trigger) {
        observer = new IntersectionObserver(function (entries) {
            if (entries[0].isIntersecting) {
                loadProducts();
            }
        }, { threshold: 1.0 });
        observer.observe(trigger);
    }
});

