function AddToCart(itemId) {
    var username = document.cookie;
    if (sessionStorage.getItem("cart") == null) {
        sessionStorage.setItem("cart", JSON.stringify([]));
    }
    let cart = JSON.parse(sessionStorage.getItem("cart"));
    let cartItemId = cart.findIndex(item => item.id === itemId);
    if (cartItemId === -1) {
        cart.push({ id: itemId, quan: Number($('#quantity').val()) })
    } else {
        cart[cartItemId] = { ...cart[cartItemId], quan: Number(cart[cartItemId].quan) + Number($('#quantity').val()) };
    }
    sessionStorage.setItem("cart", JSON.stringify(cart));
}

$(function () {
    $('.add-to-cart').click(function () {
        $.ajax({
            url: '/Products/AddItemToCart',
            data: {
                'itemId': $('#itemId').val(),
                'amount': $('#quantity').val(),
            }
        }).done(function (data) {
        });
    });
});