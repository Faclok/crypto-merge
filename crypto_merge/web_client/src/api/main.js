import { HttpData, HttpNoData } from './httpMethods';

// use
export async function SearchTransaction(cc) {

    const response = await HttpNoData('/api/transactions?idCC=' + cc, 'GET');

    return response.data;
}

// use
export async function SearchUser(chatId) {
    
    const response = await HttpNoData('/api/users/' + chatId, 'GET');

    return response.data;
}

// use
export async function updateBoostWallet(walletId, sum, currency) {
    
    const response = await HttpNoData(`/api/wallets/${walletId}/boost/${sum}/${currency}`, 'PUT');

    return response.data;
}

// use
export async function updateWalletAccountCryptoCardId(walletId, accountCryptoCardId) {
    
    const response = await HttpNoData(`/api/wallets/${walletId}/accountCryptoCardId/${accountCryptoCardId}`, 'PUT');

    return response.data;
}

// use
export async function getCryptoCards() {
    
    const response = await HttpNoData(`/api/accountCryptoCards`, 'GET');

    return response.data;
}

// use
export async function updateCryptoCardIdWallet(walletId, ccId) {
    
    const response = await HttpNoData(`/api/wallets/${walletId}/ccid/${ccId}`, 'PUT');

    return response.data;
}

// use
export async function postSetBalance(chatId, type, sum, comment) {
    
    const response = type == 2 ? await HttpNoData(`/api/users/${chatId}/newBalance/${sum}?comment=${comment}`, "POST") 
    : await HttpNoData(`/api/users/${chatId}/upBalance/${sum}?comment=${comment}`, 'POST');

    return response.data;
}

// use
export async function GetWallets(chatId) {
    
    const response = await HttpNoData('/api/wallets/all/' + chatId, 'GET');

    return response.data;
}

// use
export async function postSendMessage(chatId, text) {
    
    const response = await HttpData(`/api/message/chat/${chatId}?message=${text}`, 'POST');

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

// use
export async function postNoteAdmin(chatId, text) {
    
    const response = await HttpNoData(`/api/users/${chatId}/note/${text}`, 'PUT');

    return response.data;
}


// use
export async function searchUserOnRequest(request) {
    
    const response = await HttpNoData('/api/users/search?requisites=' + request, 'GET');

    return response.data;
}

