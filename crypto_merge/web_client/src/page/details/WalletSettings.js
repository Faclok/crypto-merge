import { useEffect, useState } from "react";
import { Alert, Button, Col, Form, Modal, Row } from "react-bootstrap";
import { PiWarningOctagon } from "react-icons/pi";
import styles from './WalletSettings.module.css';
import { deletedWalletAPI, searchWalletsOnProperties, sendMessageOnWallet, stopWalletAPI } from "../../api/details";

function WalletSettings({ show, onHide, searchProperties }) {

    const [wallets, setWallets] = useState([]);

    useEffect(() => {

        async function DidMount() {
            
            const response = await searchWalletsOnProperties(searchProperties.login, searchProperties.numberCard, searchProperties.phone, searchProperties.chatId);

            setWallets(response ? [response] : []);
        }

        DidMount();

    }, []);

    const WalletItem = ({ item }) => {

        const [isSend, setIsSend] = useState(item.isSend);
        const [isStop, setIsStop] = useState(item.isStop);
        const [isDeleted, setIsDeleted] = useState(item.isDeleted);

        const sendMessage = async() => {
            const response = await sendMessageOnWallet(item.id, 'Обратитесь в поддержку бота');
            setIsSend(item.isSend = true);
        }

        const stopWallet = async() => {
            const response = await stopWalletAPI(item.id);
            setIsStop(item.isStop = true);
        }

        const deleteWallet = async() => {
            const response = await deletedWalletAPI(item.id);
            setIsDeleted(item.isDeleted = true);
        }

        return <Form className={styles.walletItem + ' p-2'}>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Телефон:
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.phone} />
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Номер карты:
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.numberCard} />
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    логин:
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.login} />
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    пароль:
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.password} />
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Банк:
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.bank} />
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3 d-grid gap-2" controlId="formPlaintextEmail">
                    <Button variant={(isSend ? 'outline-': '') + 'success'} onClick={sendMessage}>Уведомить в чат</Button>
                    <Button variant={(isStop ? 'outline-': '') + "warning"} onClick={stopWallet}>Остановить</Button>
                    <Button variant={(isDeleted ? 'outline-': '') + "danger"} onClick={deleteWallet}>Удалить</Button>
            </Form.Group>
        </Form>
    }

    return <Modal
        show={show}
        onHide={onHide}
        size="lg"
        aria-labelledby="contained-modal-title-vcenter"
        centered>
        <Form>
            <Modal.Header closeButton>
                <Modal.Title>Реквизиты</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {
                    wallets.length <= 0 ?
                        <Alert variant="warning" style={{ display: 'flex', alignItems: 'center' }}><PiWarningOctagon style={{ marginRight: 8 }} /> У пользователя нет реквизитов</Alert>
                        : wallets
                            .map((o, i) => <WalletItem key={i} item={o} />)
                }
            </Modal.Body>
        </Form>
    </Modal>
}

export default WalletSettings;