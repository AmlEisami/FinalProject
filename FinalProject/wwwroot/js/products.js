function changeQuantity(itemId) {
    $.ajax({
        url: '/Products/ChangeQuantity',
        data: {
            'itemId': itemId,
            'amount': $('#cartQuantity_' + itemId).val(),
        }
    });
};

function removeItemFromCart(itemId) {
    $.ajax({
        url: '/Products/RemoveItemFromCart',
        data: {
            'itemId': itemId,
        }
    }).done(function (data) {
        location.reload();
    });
};

$(function () {
    $('.add-to-cart').click(function () {
        $.ajax({
            url: '/Products/AddItemToCart',
            data: {
                'itemId': $('#itemId').val(),
                'amount': $('#quantity').val(),
            }
        }).done(function () {
            var toastHTMLElement = $('#liveToast');
            var toastElement = new bootstrap.Toast(toastHTMLElement);
            toastElement.show();
            $('#quantity').val("0");
        });
    });

    $('#createOrderButtonForm').submit(function () {
        event.preventDefault();
        $.ajax({
            url: '/Orders/CreateOrder',
            data: {
                'address': $('#address').val(),
            }
        }).done(function (isSuccess) {
            $('#cartEmpty').attr("hidden", isSuccess);
            if (isSuccess) {
                location.pathname = '/Orders/MyOrders';
            }
        });
    });
});