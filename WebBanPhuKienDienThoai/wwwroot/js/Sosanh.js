$(document).ready(function () {
    var selectedProducts = [];

    $('.compare').on('click', function () {
        var productId = $(this).data('productid');
        var productName = $(this).data('name');
        var productImage = $(this).data('image');
        if (!selectedProducts.some(p => p.id === productId)) {
            selectedProducts.push({ id: productId, name: productName, image: productImage });
            updateCompareSection();
        }
    });

    function updateCompareSection() {
        var compareSection = $('#compare-section');
        compareSection.empty();
        selectedProducts.forEach(function (product, index) {
            compareSection.append(`
                        <div class="compare-item">
                            <img src="${product.image}" alt="${product.name}" style="width: 50px; height: 50px;" />
                            <span>${product.name}</span>
                            <button class="remove-compare" data-index="${index}">×</button>
                        </div>
                    `);
        });
        if (selectedProducts.length > 0) {
            compareSection.append(`
                        <div class="compare-actions">
                            <button id="compare-now" class="btn btn-primary">@Localizer["Compare Now"]</button>
                            <button id="clear-compare" class="btn btn-danger">@Localizer["Clear All"]</button>
                        </div>
                    `);
        }
    }

    $(document).on('click', '.remove-compare', function () {
        var index = $(this).data('index');
        selectedProducts.splice(index, 1);
        updateCompareSection();
    });

    $(document).on('click', '#clear-compare', function () {
        selectedProducts = [];
        updateCompareSection();
    });

    $(document).on('click', '#compare-now', function () {
        if (selectedProducts.length > 0) {
            var productIds = selectedProducts.map(p => p.id).join(',');
            window.location.href = '/ShoppingCart/CompareProducts?productIds=' + productIds;
        } else {
            alert('@Localizer["Please select at least one product to compare!"]');
        }
    });
});
// Hàm lấy token (cần @Html.AntiForgeryToken() trong _Layout.cshtml)
function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}
