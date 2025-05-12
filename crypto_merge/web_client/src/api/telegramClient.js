import './httpMethods';
import { HttpNoData } from './httpMethods';

export async function getStatus(numberBot){

    const response = await HttpNoData(`/api/telegramClient/status/${numberBot}`, 'GET');

    return response.data;
}

export async function sendCode(code, numberBot){

    const response = await HttpNoData(`/api/telegramClient/code/${numberBot}/${code}`, 'POST');

    return response;
}