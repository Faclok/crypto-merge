import { HttpData, HttpNoData } from "./httpMethods";

// use
export async function GetDeposits(){

    const response = await HttpNoData('/api/deposits', 'GET');

    return response.data;
}

// use
export async function GetDepositsOnAccountCC(accountCryptoCardId){

    const response = await HttpNoData(`/api/deposits/accountCryptoCards/${accountCryptoCardId}`, 'GET');

    return response.data;
}

export async function putSumTransaction(transactionId, sum) {
    
    const response = await HttpNoData(`/api/transactions/${transactionId}?sum=${sum}`, 'PUT');

    return response.data;
}

// use
export async function PutStatus(id, isCompleted, isClose){

    const response = await HttpNoData(`/api/deposits/${id}/status?isCompleted=${isCompleted}&isClose=${isClose}`, 'PUT');

    return response.data;
}

// use
export async function sendMessageTransaction(transactionId, text, tag) {

    const response = await HttpData(`/api/message/transactions/${transactionId}?tag=${tag}`, "POST", null, {
        message: text
    });

    return response.data;
}

// use
export async function sendMessageRequestTransaction(transactionId, text, titleRequest, request, titleSum, sum, comment, tag) {

    const response = await HttpData(`/api/message/transactions/${transactionId}/request?tag=${tag}`, "POST", null, {
        titleRequest: titleRequest,
        request: request,
        titleSum: titleSum,
        sum: sum,
        comment: comment,
        message: text
    });

    return response.data;
}

// api
export async function GetMessages(transactionId) {
    
    const response = await HttpNoData(`/api/message/transactions/${transactionId}`, 'GET');

    return response.data;
}

//
export async function GetBalanceOnTransaction(transactionId) {
    
    const response = await HttpNoData(`/api/wallets/balance/transactions/${transactionId}`, 'GET');

    return response.data;
}