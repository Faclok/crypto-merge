import { useEffect, useState } from "react";
import { Button, Col, Form, InputGroup, Modal, Row, Stack } from "react-bootstrap";
import WalletEditWindow from "./WalletEditWindow";
import WalletSettings from './WalletSettings';
import { postNoteAdmin, SearchUser, searchUserOnRequest } from "../../api/main";

function UserWindow({ show, onHide, request, type }) {

    const [user, setUser] = useState();
    const [isShowBalanceWindow, setIsShowBalanceWindow] = useState(false);
    const [isShowWalletsWindow, setIsShowWalletsWindow] = useState(false);

    useEffect(() => {

        async function DidMount(){

            const response = type == 'request' ? await searchUserOnRequest(request) : await SearchUser(request);

            setUser(response);
        }

        DidMount();
    }, []);

    if(!user)
        return <Modal show={show} onHide={onHide} size="sm">
        <Modal.Header closeButton>
            <Modal.Title>Упс...</Modal.Title>
        </Modal.Header>
        <Modal.Body>
            <p>Поиск ничего не нашел</p>
        </Modal.Body>
    </Modal>

    if(isShowBalanceWindow)
        return <WalletEditWindow balance={user.balance} chatId={user.chatId} setBalance={(v) => user.balance = v} show={isShowBalanceWindow} onHide={() => setIsShowBalanceWindow(false)}/>;

    if(isShowWalletsWindow)
        return <WalletSettings chatId={user.chatId} show={isShowWalletsWindow} onHide={() => setIsShowWalletsWindow(false)}/>;

    return <Modal show={show} onHide={onHide}
        size="lg"
        aria-labelledby="contained-modal-title-vcenter"
        centered>
        <Modal.Header closeButton>
            <Modal.Title>{user.chatId} • {user.username}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
            <Form>
                {
                    user.countWallets > 0 ? 
                    <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                    <Form.Label column sm="3">
                        Баланс:
                    </Form.Label>
                    <Col sm="9">
                        <Button onClick={() => setIsShowBalanceWindow(true)}>{user.balance}</Button>
                    </Col>
                </Form.Group> : ''
                }
                <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                    <Form.Label column sm="3">
                        Статистика:
                    </Form.Label>
                    <Col sm="9">
                        <Form.Control plaintext readOnly value={user.statisticsAll} />
                    </Col>
                </Form.Group>
                <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                    <Form.Label column sm="3">
                        Статистика за 3 дня:
                    </Form.Label>
                    <Col sm="9">
                        <Form.Control plaintext readOnly value={user.statistics3Days} />
                    </Col>
                </Form.Group>
                <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                    <Form.Label column sm="3">
                        Кол-во банков:
                    </Form.Label>
                    <Col sm="9">
                        <Button onClick={() => setIsShowWalletsWindow(true)}>{user.countWallets}</Button>
                    </Col>
                </Form.Group>
                <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                    <Form.Label column sm="3">
                        Доход от рефералов:
                    </Form.Label>
                    <Col sm="9">
                        <Form.Control plaintext readOnly value={user.incomeRefUsers} />
                    </Col>
                </Form.Group>
                <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                    <Form.Label column sm="3">
                        Заметка:
                    </Form.Label>
                    <Col sm="9">
                    <Stack gap={1}>
                        <Form.Control as="textarea" value={user.noteAdmin} onChange={(e) => setUser({ ...user, noteAdmin: e.target.value })} />
                        <Button onClick={async() => {
                            await postNoteAdmin(user.chatId, user.noteAdmin);
                        }}>Сохранить</Button>
                    </Stack>
                    </Col>
                </Form.Group>
            </Form>
        </Modal.Body>
    </Modal>
}

export default UserWindow;