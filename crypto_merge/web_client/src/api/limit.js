import { HttpNoData } from "./httpMethods";

// use
export async function getCountLimits() {
    
    const response = await HttpNoData('/api/limits/count');

    return response.data;
}


// use
export async function getLimits() {
    
    const response = await HttpNoData('/api/limits');

    return response.data;
}

export async function PutCheck(walletId) {
 
    const response = await HttpNoData('/api/limits/'+walletId, 'PUT');

    return response.data;
}