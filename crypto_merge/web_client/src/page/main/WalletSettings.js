import { useEffect, useState } from "react";
import { Alert, Button, Col, Form, Modal, Row } from "react-bootstrap";
import { PiWarningOctagon } from "react-icons/pi";
import styles from './MainPage.module.css';
import { deletedWalletAPI, getCryptoCards, GetWallets, postSendMessage, stopWalletAPI, updateBoostWallet, updateCryptoCardIdWallet, updateWalletAccountCryptoCardId } from "../../api/main";


function WalletSettings({ show, chatId, onHide }) {

    const [wallets, setWallets] = useState([]);

    const [cryptoCards, setCryptoCards] = useState([]);

    useEffect(() => {

        async function DidMount() {
            
            const ccs = [{id: -1, name: "Все"}, ...((await getCryptoCards()) ?? [])];
            const response = await GetWallets(chatId);

            setCryptoCards(ccs);
            setWallets(response ?? []);
        }

        DidMount();

    }, []);

    const WalletItem = ({ item }) => {

        const [isSend, setIsSend] = useState(item.isSend);
        const [isStop, setIsStop] = useState(item.isStop);
        const [isDeleted, setIsDeleted] = useState(item.isDeleted);
        const [isEditBoost, setIsBoost] = useState(false);
        const [editBoost, setEditBoost] = useState(item.balanceBoost);

        const [isEditCCID, setIsEditCCID] = useState(false);
        const [ccID, setCcID] = useState(item.cryptoCardId);
        const [accountCryptoCardId, setAccountCryptoCardId] = useState(item.accountCryptoCardId);

        const sendMessage = async() => {
            const response = await postSendMessage(chatId, 'Обратитесь в поддержку бота');
            setIsSend(item.isSend = true);
        }

        const updateBoost = async() => {
            const response = await updateBoostWallet(item.id, editBoost, "USD");
            item.balanceBoost = editBoost;
            setIsBoost(false);
        }

        const stopWallet = async() => {
            const response = await stopWalletAPI(item.id);
            setIsStop(item.isStop = true);
        }

        const deleteWallet = async() => {
            const response = await deletedWalletAPI(item.id);
            setIsDeleted(item.isDeleted = true);
        }

        const updateCCID = async() => {
            const response = await updateCryptoCardIdWallet(item.id, ccID);
            item.cryptoCardId = ccID;
            setIsEditCCID(false);
        }

        if(isEditCCID)
            return <Form className={styles.walletItem + ' p-2'}>
            <Form.Group as={Row} className="mb-3" controlId="formCC">
                <Form.Label column sm="2">
                    CC ID:
                </Form.Label>
                <Col sm="10">
                    <Form.Control value={ccID} onChange={(e) => setCcID(e.target.value)} type="text"/>
                </Col>
                <Button onClick={() => updateCCID()}>Сохранить</Button>
            </Form.Group>
        </Form>;

        if(isEditBoost)
            return <Form className={styles.walletItem + ' p-2'}>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextPhone">
                <Form.Label column sm="2">
                    Буст тык:
                </Form.Label>
                <Col sm="10">
                    <Form.Control value={editBoost} onChange={(e) => setEditBoost(e.target.value)} type="number"/>
                </Col>
                <Button onClick={() => updateBoost()}>Сохранить</Button>
            </Form.Group>
        </Form>;

        return <Form className={styles.walletItem + ' p-2'}>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Телефон:
                </Form.Label>
                <Col sm="10">
                    <Button onClick={() => setIsBoost(true)}>{item.phone}</Button>
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    CC ID:
                </Form.Label>
                <Col sm="10">
                    <Button onClick={() => setIsEditCCID(true)}>{item.cryptoCardId}</Button>
                </Col>
            </Form.Group>

            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Статус:
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.status}/>
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    CC Аккаунт:
                </Form.Label>
                <Col sm="10">
                    <Form.Select aria-label="Default select example" value={accountCryptoCardId ?? -1} onChange={async(e) => {

                            const newValue = e.target.value == -1 ? null : e.target.value;

                            if(!newValue){
                                alert("К сожалению нельзя поставить на несуществующую платформу");
                                return;
                            }

                            await updateWalletAccountCryptoCardId(item.id, newValue);
                            item.accountCryptoCardId = newValue;
                            setAccountCryptoCardId(newValue);
                    }}>{
                        cryptoCards && cryptoCards.length > 0 ? 
                         cryptoCards.map((o, i) => <option key={i} value={o.id}>{o.name}</option>) : ''
                        }</Form.Select>
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
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Тык:
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.tuk} />
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Буст Тык:
                </Form.Label>
                <Col sm="10">
                <Form.Control plaintext readOnly value={item.boostTuk} />
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Буст тык:
                </Form.Label>
                <Col sm="10">
                    <Button onClick={() => setIsBoost(true)}>{item.balanceBoost}</Button>
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
                            .map(o => <WalletItem key={o.id} item={o} />)
                }
            </Modal.Body>
        </Form>
    </Modal>
}

export default WalletSettings;