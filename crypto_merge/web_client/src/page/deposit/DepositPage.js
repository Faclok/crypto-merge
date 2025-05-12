import Button from 'react-bootstrap/Button';
import styles from './DepositPage.module.css';
import { useEffect, useState } from 'react';
import { Alert, Col, Container, Form, Offcanvas, Row, Stack } from 'react-bootstrap';
import MessageWindow from './MessageWindow';
import { FaRegCopy } from 'react-icons/fa6';
import { GetDeposits, GetDepositsOnAccountCC, PutStatus } from '../../api/deposit';
import { getCryptoCards } from '../../api/main';

function DepositPage() {

    document.title = "Пополнения";

    const [deposits, setDeposits] = useState([]);
    const [cryptoCards, setCryptoCards] = useState([]);
    const [accountCryptoCardId, setAccountCryptoCardId] = useState(null);
    const [tick, setTick] = useState(false);
    const [isTick, setIsTick] = useState(false);

    async function DidMount() {

        const elements = !accountCryptoCardId ? await GetDeposits() : await GetDepositsOnAccountCC(accountCryptoCardId);

        setDeposits(elements ?? []);
    }

    async function DidMountCCS() {

        setCryptoCards([{id: -1, name: "Все"}, ...((await getCryptoCards()) ?? [])]);
    }

    useEffect(() => {

        DidMountCCS();
        DidMount();
    }, []);

    useEffect(() => {

        if (!isTick)
            return;

        const timerID = setInterval(() => setTick(!tick), 3000);
        DidMount();
        return () => clearInterval(timerID);
    }, [isTick, tick]);

    const DepositItem = ({ deposit }) => {

        const [show, setShow] = useState(false);
        const [status, setStatus] = useState('wait');
        const [copies, setCopies] = useState([]);
        const handleClose = () => setShow(false);
        const handleShow = () => setShow(true);

        useEffect(() => {

            async function DidMount() {
                if (status == 'completed') {
                    const responseBalance = await PutStatus(deposit.transactionId, true, false);
                    setCopies(responseBalance ?? []);
                }
                else if (status == 'removeView') {
                    setDeposits(deposits.filter(o => o.transactionId != deposit.transactionId));
                } else if (status == 'deleted') {
                    await PutStatus(deposit.transactionId, false, true);
                    setDeposits(deposits.filter(o => o.transactionId != deposit.transactionId));
                }
            }

            DidMount();
        }, [status]);

        return <div>
            <div className={styles.depositItem}>
                <h6>ID: {deposit.chatId}</h6>
                <h6>USERNAME: {deposit.username}</h6>
                <h6>CC ID: {deposit.cryptoCardId}</h6>
                <h6>Сумма: {deposit.countTransaction}</h6>
                <h6>Банки: {deposit.banks?.join('; ')}</h6>
                <h6>Баланс: {deposit.balance}</h6>
                <h6>Буст тык.: {deposit.balanceBoost}</h6>
                <Button variant='success' onClick={() => handleShow()}>Открыть</Button>
                {
                    status == 'completed' ?
                        <Button variant='primary' onClick={() => setStatus('removeView')}>Закрыть</Button>
                        : ''
                }
            </div>
            <Offcanvas show={show} onHide={handleClose}>
                <Offcanvas.Header closeButton>
                    <Offcanvas.Title>ID: {deposit.chatId} | CC: {deposit.cryptoCardId}</Offcanvas.Title>
                </Offcanvas.Header>
                <Offcanvas.Body>
                    <Container className='d-grid gap-2'>
                        <Row>
                            <Col>
                                <Stack gap={3}>
                                    <h6>USERNAME: {deposit.username}</h6>
                                    <h6>Сумма: {deposit.countTransaction}</h6>
                                    <h6>Банки: {deposit.banks?.join('; ')}</h6>
                                    <h6>Баланс: {deposit.balance}</h6>
                                    <h6>Буст тык.: {deposit.balanceBoost}</h6>
                                </Stack>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <MessageWindow transaction={deposit} transactionId={deposit.transactionId} />
                            </Col>
                        </Row>
                        {
                            status == 'completed' ?
                                <Row style={{ marginTop: 20 }}>
                                    <Col>
                                        {
                                            copies.length > 0 ?
                                                copies.map((o, i) => <Alert variant='light' key={i} className={styles.copyItem} onClick={() => navigator.clipboard.writeText((+o.value).toFixed(2))}>
                                                    <div>{o.name}: {(+o.value).toFixed(2)}</div>
                                                    <FaRegCopy />
                                                </Alert>) : ''
                                        }
                                    </Col>
                                </Row> :
                                <Row style={{ marginTop: 20 }}>
                                    <Col className='d-grid gap-2'>
                                        <Button variant='success' onClick={() => setStatus('completed')}>Завершить</Button>
                                    </Col>
                                    <Col className='d-grid gap-2'>
                                        <Button variant='danger' onClick={() => setStatus('deleted')}>Отклонить</Button>
                                    </Col>
                                </Row>
                        }
                    </Container>
                </Offcanvas.Body>
            </Offcanvas>
        </div>
    }

    if (deposits.length <= 0)
        return <div>
            <Form style={{marginBottom: 40}}>
            <Form.Select aria-label="Default select example" value={accountCryptoCardId ?? -1} onChange={(e) => {

                setAccountCryptoCardId(e.target.value == -1 ? null : e.target.value);
            }}>{
                    cryptoCards && cryptoCards.length > 0 ?
                        cryptoCards.map((o, i) => <option key={i} value={o.id}>{o.name}</option>) : ''
                }</Form.Select>
        </Form>
             <Alert variant='warning' style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
            <span>Заявок пока что нет</span>
            <Stack gap={3} direction="horizontal">
                <Button variant="warning" onClick={DidMount}>Обновить</Button>
                <Button variant="warning" onClick={() => setIsTick(!isTick)}>{isTick ? "Выключить" : "Включить"} таймер</Button>
            </Stack>
        </Alert>
        </div>

    return <div>
        <Form style={{marginBottom: 40}}>
            <Form.Select aria-label="Default select example" value={accountCryptoCardId ?? -1} onChange={async (e) => {
                const newValue = e.target.value == -1 ? null : e.target.value;

                if (!newValue) {
                    alert("К сожалению нельзя поставить на несуществующую платформу");
                    return;
                }

                setAccountCryptoCardId(newValue);
            }}>{
                    cryptoCards && cryptoCards.length > 0 ?
                        cryptoCards.map((o, i) => <option key={i} value={o.id}>{o.name}</option>) : ''
                }</Form.Select>
        </Form>
        {
            deposits.map(o => <DepositItem key={o.id} deposit={o} />)
        }
        <Stack gap={3} direction="horizontal">
            <Button variant="warning" onClick={DidMount}>Обновить</Button>
            <Button variant="warning" onClick={() => setIsTick(!isTick)}>{isTick ? "Выключить" : "Включить"} таймер</Button>
        </Stack>
    </div>;
}

export default DepositPage;