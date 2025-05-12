import { Form, Row, Col, Button } from "react-bootstrap";
import TransferWindow from "./TransferWindow";
import { useState } from "react";

function TransferSearch(){

    const [request, setRequest] = useState('');
    const [cc, setCC] = useState('');
    const [chatId, setChatId] = useState('');
    const [sum, setSum] = useState('');
    const [isVisible, setIsVisible] = useState(false);

    return <div>
        <h2>Функция поиска перевода</h2>
        <Form onSubmit={(e) => setIsVisible(true) || e.preventDefault()}>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Реквизиты
                </Form.Label>
                <Col sm="10">
                    <Form.Control value={request} onChange={(e) => setRequest(e.target.value)} type="number" placeholder="XXXX-XXXX-XXXX-XXXX" />
                </Col>
            </Form.Group>

            <Form.Group as={Row} className="mb-3" controlId="formPlaintextPassword">
                <Form.Label column sm="2">
                    ID перевода
                </Form.Label>
                <Col sm="10">
                    <Form.Control value={cc} onChange={(e) => setCC(e.target.value)} type="number" placeholder="XXXX-XXXX-XXXX-XXXX" />
                </Col>
            </Form.Group>

            <Form.Group as={Row} className="mb-3" controlId="formPlaintextPassword">
                <Form.Label column sm="2">
                    ID пользователя
                </Form.Label>
                <Col sm="10">
                    <Form.Control value={chatId} onChange={(e) => setChatId(e.target.value)} type="number" placeholder="433231" />
                </Col>
            </Form.Group>

            <Form.Group as={Row} className="mb-3" controlId="formPlaintextPassword">
                <Form.Label column sm="2">
                     Сумма
                </Form.Label>
                <Col sm="10">
                    <Form.Control value={sum} onChange={(e) => setSum(e.target.value)} type="text" placeholder="000000000" />
                </Col>
            </Form.Group>
            <Button type="submit" variant="success">Поиск</Button>
        </Form>
        {
            isVisible ? 
            <TransferWindow show={isVisible} onHide={() => setIsVisible(false)} properties={{
                request: request == '' ? null : request,
                cc: cc == '' ? null : cc,
                chatId: chatId == '' ? null : chatId,
                sum: sum == '' ? null : sum
            }}/>: ''
        }
    </div>
}

export default TransferSearch;