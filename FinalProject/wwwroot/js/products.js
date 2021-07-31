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
        });
    });

    $('#cartQuantity').change(function () {
        $.ajax({
            url: '/Products/ChangeQuantity',
            data: {
                'itemId': $('#cartItemId').val(),
                'amount': $('#cartQuantity').val(),
            }
        });
    });

    $('#createOrderButtonForm').submit(function () {
        event.preventDefault();
        $.ajax({
            url: '/Orders/CreateOrder',
            data: {
                'address': $('#address').val(),
            }
        });
    });
});