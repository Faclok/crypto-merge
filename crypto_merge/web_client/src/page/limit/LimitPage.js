import { useEffect, useState } from "react";
import { Alert, Button, Col, Form, InputGroup, Row, Stack } from "react-bootstrap";
import { getLimits, PutCheck } from "../../api/limit";
import { BiCopy } from "react-icons/bi";
import { IoCloudDone } from "react-icons/io5";
import { getCryptoCards } from "../../api/main";

function LimitPage({ title }) {

    document.title = title;

    const [orders, setOrders] = useState([]);
    const [accountCryptoCards, setAccountCryptoCards] = useState([]);

    useEffect(() => {

        async function didMount() {

            setAccountCryptoCards([{id: -1, name: "Все"}, ...((await getCryptoCards()) ?? [])]);
            setOrders((await getLimits()) ?? []);
        }

        didMount();
    }, []);

    const handler = async(id) => {
        await PutCheck(id);

        setOrders(orders.filter(o => o.id != id));
    }

    return <div>
        {
            orders.length <= 0 ?
                <Alert variant="success">На сегодня все</Alert>
                : <Stack gap={5}>
                    {
                        orders.map(o => <Row>
                            <Col>
                            <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
                                <Form.Label>ID <BiCopy style={{cursor: 'pointer'}} onClick={() => navigator.clipboard.writeText(o.chatId)}/></Form.Label>
                                <Form.Control readOnly disabled value={o.chatId} />
                            </Form.Group>
                            </Col>
                            <Col>
                            <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
                                <Form.Label>Телефон <BiCopy style={{cursor: 'pointer'}} onClick={() => navigator.clipboard.writeText(o.phone)}/></Form.Label>
                                <Form.Control readOnly disabled value={o.phone}/>
                            </Form.Group>
                            </Col>
                            <Col>
                            <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
                                <Form.Label>Карта <BiCopy style={{cursor: 'pointer'}} onClick={() => navigator.clipboard.writeText(o.numberCard)}/></Form.Label>
                                <Form.Control readOnly disabled value={o.numberCard} />
                            </Form.Group>
                            </Col>
                            <Col>
                            <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
                                <Form.Label>CC Аккаунт</Form.Label>
                                <Form.Control readOnly disabled value={(accountCryptoCards.find(f => f.id == o.accountCryptoCardId)?.name) ?? (accountCryptoCards.find(f => f.id == -1).name)} />
                            </Form.Group>
                            </Col>
                            <Col>
                            <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
                                <Form.Label>Итог <BiCopy style={{cursor: 'pointer'}} onClick={() => navigator.clipboard.writeText((+o.balance).toFixed(2))}/></Form.Label>
                                <Form.Control readOnly disabled value={(+o.balance).toFixed(2)} />
                            </Form.Group>
                            </Col>
                            <Col className="d-grid gap-2 p-3">
                                <Button onClick={() => handler(o.id)}>Зафиксировать <IoCloudDone/></Button>
                            </Col>
                        </Row>)
                    }
                </Stack>
        }
    </div>
}

export default LimitPage;