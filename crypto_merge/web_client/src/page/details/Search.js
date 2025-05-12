import React, { useState } from "react";
import { Col, Row, Form, Button, Modal } from "react-bootstrap";
import WalletSettings from "./WalletSettings";

function Search({ onDataSearch }) {

    const [isVisible, setVisible] = useState(false);
    const [userSearch, setUserSearch] = useState();

    const [login, setLogin] = useState('');
    const [numberCard, setNumberCard] = useState('');
    const [phone, setPhone] = useState('');
    const [chatId, setChatId] = useState('');

    return <div>
        <h1>Поиск по реквизитам</h1>
        <Form>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Логин
                </Form.Label>
                <Col sm="10">
                    <Form.Control type="email" placeholder="email@example.com" value={login} onChange={(e) => setLogin(e.target.value)}/>
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextPassword">
                <Form.Label column sm="2">
                    Номер карты
                </Form.Label>
                <Col sm="10">
                    <Form.Control type="number" placeholder="XXXX-XXXX-XXXX-XXXX" value={numberCard} onChange={(e) => setNumberCard(e.target.value)}/>
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextPassword">
                <Form.Label column sm="2">
                    Номер телефона
                </Form.Label>
                <Col sm="10">
                    <Form.Control type="phone" placeholder="+7904 000 00 00" value={phone} onChange={(e) => setPhone(e.target.value)}/>
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextPassword">
                <Form.Label column sm="2">
                    ID пользователя
                </Form.Label>
                <Col sm="10">
                    <Form.Control type="text" placeholder="000000000" value={chatId} onChange={(e) => setChatId(e.target.value)}/>
                </Col>
            </Form.Group>
            <Button variant="success" onClick={() => setUserSearch({ chatId: chatId, phone: phone, numberCard: numberCard, login: login }) || setVisible(true)}>Поиск</Button>
        </Form>
        {
            isVisible ? 
            <WalletSettings show={isVisible} searchProperties={userSearch} onHide={() => setVisible(false)}/>
            : ''
        }
    </div>
}

export default Search;