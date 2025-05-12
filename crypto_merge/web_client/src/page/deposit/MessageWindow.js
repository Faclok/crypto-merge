import { memo, useState } from "react";
import { Alert, Button, Form, Image, InputGroup, Stack } from "react-bootstrap";
import { IoSend } from 'react-icons/io5';
import styles from './MessageWindow.module.css';
import { GetMessages, putSumTransaction, sendMessageRequestTransaction, sendMessageTransaction } from "../../api/deposit";

function MessageWindow({ transaction , transactionId }) {

    const [messages, setMessages] = useState(transaction.chat ?? []);

    const messageSend = async (text, tag) => {

        if (!text || text.trim() == '') {
            alert('Поле не может быть пустым!');
            return;
        }

        const response = await sendMessageTransaction(transactionId, text, tag);

        const newChat = [...messages, { id: response, message: text, isUser: false, tag: tag }];
        setMessages(newChat);
        transaction.chat = newChat;

        return true;
    };

    const messageSendRequest = async (text, titleRequest, request, titleSum, sum, comment, tag) => {

        if (!text || text.trim() == '') {
            alert('Поле не может быть пустым!');
            return;
        }

        const response = await sendMessageRequestTransaction(transactionId, text, titleRequest, request, titleSum, sum, comment, tag);

        const newChat = [...messages, { id: response, message: text, isUser: false, tag: tag }];
        setMessages(newChat);
        transaction.chat = newChat;

        return true;
    };

    const MessageItem = ({ message }) => {

        const [isFull, setIsFull] = useState(false);

        return <div style={{ whiteSpace: "pre-wrap" }} className={(!message.isUser ? styles.messageAdmin : styles.messageUser) + ' p-2'} key={message.id}>
            {
                message.file ?
                    <Image
                        fluid
                        src={'data:image/jpeg;base64,' + message.file.data}
                        alt="screen"
                        onClick={() => setIsFull(!isFull)}
                        style={isFull ? {
                            position: 'absolute',
                            top: 0,
                            left: 0,
                            right: 0,
                            bottom: 0,
                            height: '100%',
                            width: '100%',
                            objectFit: 'contain',
                        }: {}}
                    /> : ''
            }
            {message.message}
        </div>;
    };

    const InputUser = memo(function InputUser({ isRequest }) {

        const MessageBankRequest = () => {

            const [requestData, setRequestData] = useState('');
            const [sum, setSum] = useState('');
            const [comment, setComment] = useState('');
            const [validated, setValidated] = useState(false);

            const handleSubmit = async (event) => {
                const form = event.currentTarget;
                event.preventDefault();

                if (form.checkValidity() === false) {
                    event.stopPropagation();
                } else {
                    await putSumTransaction(transactionId, sum); 
                    await messageSendRequest(`Реквизиты: ${requestData}\nСумма: ${sum}` + (!comment ? '' : `\nКомментарий: ${comment}`), `Реквизиты`, requestData, 'Сумма', sum, comment, 'requestData');

                    transaction.countTransaction = sum;
                }

                setValidated(true);
            };

            return <Form noValidate validated={validated} onSubmit={handleSubmit}>
                <Form.Group className="mb-3">
                    <Form.Label>Реквизиты</Form.Label>
                    <Form.Control required value={requestData} onChange={(e) => setRequestData(e.target.value)} placeholder="Sber - XXXX XXXX XXXX XXXX" aria-label="Large" aria-describedby="inputGroup-sizing-sm" />
                    <Form.Control.Feedback>Верно</Form.Control.Feedback>
                    <Form.Control.Feedback type="invalid">Реквизиты должны быть заполнены!</Form.Control.Feedback>
                </Form.Group>

                <Form.Group className="mb-3">
                    <Form.Label>Сумма</Form.Label>
                    <Form.Control required value={sum} onChange={(e) => setSum(e.target.value)} type="number" placeholder="10000" />
                    <Form.Control.Feedback>Верно</Form.Control.Feedback>
                    <Form.Control.Feedback type="invalid">Сумма должна быть заполнена!</Form.Control.Feedback>
                </Form.Group>

                <Form.Group className="mb-3">
                    <Form.Label>Комментарий*</Form.Label>
                    <Form.Control value={comment} onChange={(e) => setComment(e.target.value)} placeholder="Не обязательно" />
                </Form.Group>

                <div className="d-grid gap-2">
                    <Button variant="primary" type="sumbit">
                        Отправить
                    </Button>
                </div>
            </Form>
        };

        const MessageSimple = () => {

            const [currentText, setCurrentText] = useState('');
            const [validated, setValidated] = useState(false);

            const handleSubmit = async (event) => {
                const form = event.currentTarget;
                event.preventDefault();

                if (form.checkValidity() === false) {
                    event.stopPropagation();
                } else {
                    await messageSend(currentText);
                }

                setValidated(true);
            };

            return <Form noValidate validated={validated} onSubmit={handleSubmit}>
                <InputGroup className="lg">
                    <Form.Control required value={currentText} onChange={(e) => setCurrentText(e.target.value)} placeholder="Сообщение" aria-label="Large" aria-describedby="inputGroup-sizing-sm" />
                    <Button variant="primary" type="sumbit" id="send_messsage">
                        <IoSend />
                    </Button>
                    <Form.Control.Feedback type="invalid">Обязательно!</Form.Control.Feedback>
                </InputGroup>
            </Form>
        };

        return isRequest ? <MessageSimple /> : <MessageBankRequest />;
    })

    return <div>
        <Stack gap={1} className={styles.chat}>
            <Button onClick={async () => setMessages(transaction.chat = (await GetMessages(transactionId)) ?? [])}>Обновить</Button>
            {
                messages.length <= 0 ?
                    <Alert variant="warning">К сожалению чат пуст</Alert> :
                    messages.map(o => <MessageItem message={o} key={o.id} />)
            }
        </Stack>
        <InputUser isRequest={transaction.chat.filter(o => o.tag == 'requestData').length > 0} />
    </div>
}

export default MessageWindow;