import { HttpData, HttpNoData } from './httpMethods.js';

// use
export async function createTransaction(body) {
    
    const response = await HttpData('/api/transaction', 'POST', null, body);

    return response.code;
}

// use
export async function searchTransactions(properties) {
    
    const response = await HttpData('/api/transaction/search', 'POST', null, properties);
    return response.data;
}

// use
export async function putSumTransaction(transactionId, sum, comment) {
    
    const response = await HttpNoData(`/api/transaction/${transactionId}/${sum}?comment=${comment}`, 'PUT');

    return response.data;
}