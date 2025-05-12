import { HttpData, HttpNoData } from "./httpMethods";

// use
export async function GetNewRequest() {

    const response = HttpNoData('/api/wallets/new', 'GET');

    return (await response).data;
}

// use
export async function connectedWalletAPI(walletId){

    const response = await HttpNoData(`/api/wallets/${walletId}/status/1`, 'PUT');

    return response.data;
}

// use
export async function resetWalletAPI(walletId){

    const response = await HttpNoData(`/api/wallets/${walletId}/reset`, 'PUT');

    return response.data;
}

// use
export async function resetWalletSMSAPI(walletId){

    const response = await HttpNoData(`/api/wallets/${walletId}/reset/sms`, 'PUT');

    return response.data;
}

// use
export async function searchWalletsOnProperties(login, numberCard, phone, id) {

    var stringRequest = [];

    if(login && login != '')
        stringRequest.push('login='+login);

    if(numberCard && numberCard != '')
        stringRequest.push('numberCard='+numberCard);

    if(phone && phone != '')
        stringRequest.push('teleNumber='+phone);

    if(id && id != '')
        stringRequest.push('id='+id);
    
    const response = await HttpData(`/api/wallets` + (stringRequest.length > 0 ? '?' + stringRequest.join('&') : ''));

    return response.data;
}

// use
export async function sendMessageOnWallet(walletId, text) {
    
    const response = await HttpNoData(`/api/wallets/${walletId}/sendMessage?text=${text}`, 'POST');

    return response.data;
}

// use
export async function stopWalletAPI(walletId){

    const response = await HttpNoData(`/api/wallets/${walletId}/status/3`, 'PUT');

    return response.data;
}

// use
export async function deletedWalletAPI(walletId) {
    
    const response = await HttpNoData('/api/wallets/'+walletId, 'DELETE');

    return response.data;
}