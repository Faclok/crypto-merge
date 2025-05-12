import { HttpNoData } from "./httpMethods";

// use
export async function getCurrency(title) {
    
    const response = await HttpNoData(`/api/currency/${title}`, 'GET');

    return response.data;
}

export async function putCurrencyRub(title, rub) {
    
    const response = await HttpNoData(`/api/currency/${title}?rub=${rub}`, 'PUT');

    return response.data;
}