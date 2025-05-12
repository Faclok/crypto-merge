import { useEffect, useState } from "react";
import { Button, Col, Form, Modal, Row } from "react-bootstrap";
import styles from './TransferWindow.module.css';
import { putSumTransaction, searchTransactions } from "../../api/transfer";


function TransferWindow({ show, onHide, properties }) {

    const [elements, setElements] = useState([]);

    useEffect(() => {

        async function DidMount() {
            
            const response = await searchTransactions(properties);
            setElements(response ?? []);
        }

        DidMount();
    }, []);

    const TransferItem = ({ item }) => {

        const [sum, setSum] = useState(item.count);
        const [isEditSum, setIsEditSum] = useState(false);

        const [sumNew, setSumNew] = useState('');
        const [comment, setComment] = useState('');

        const create = async() => {

            const response = await putSumTransaction(item.id, sumNew, comment);
            setSum(sumNew);
            setComment('');
            setSumNew('');
            setIsEditSum(false);
        };

        if(isEditSum)
            return <Form className={styles.transactionItem + ' p-3'}>
                <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Сумма
                </Form.Label>
                <Col sm="10">
                    <Form.Control value={sumNew} onChange={(e) => setSumNew(e.target.value)}/>
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Комментарий
                </Form.Label>
                <Col sm="10">
                    <Form.Control value={comment} onChange={(e) => setComment(e.target.value)}/>
                </Col>
            </Form.Group>
            <Row>
                <Col className="d-grid gap-2">
                    <Button onClick={create}>Сохранить</Button>
                </Col>
                <Col className="d-grid gap-2">
                    <Button onClick={() => setIsEditSum(false)} variant="secondary">Закрыть</Button>
                </Col>
            </Row>
            </Form>

        return <Form className={styles.transactionItem + ' p-3'}>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    СС ID
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.cc} />
                </Col>
            </Form.Group>

            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Сообщение
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.message} />
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Тип
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.type} />
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Статус
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.status} />
                </Col>
            </Form.Group>

            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Телефон
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.phone} />
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Номер карты
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.numberCard} />
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Сумма
                </Form.Label>
                <Col sm="10">
                    <Button onClick={() => setIsEditSum(true)}>{sum}</Button>
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                <Form.Label column sm="2">
                    Дата
                </Form.Label>
                <Col sm="10">
                    <Form.Control plaintext readOnly value={item.dateTimeUTC.replace('T', ' ').split('.')[0]} />
                </Col>
            </Form.Group>
        </Form>
    }

    if (elements.length <= 0)
        return <Modal show={show} onHide={onHide}
            size="lg"
            aria-labelledby="contained-modal-title-vcenter"
            centered>
            <Modal.Header closeButton>
                <Modal.Title>Упс...</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                Нет ни одного совпадения
            </Modal.Body>
        </Modal>

    return <Modal show={show} onHide={onHide}
        size="lg"
        aria-labelledby="contained-modal-title-vcenter"
        centered>
        <Modal.Header closeButton>
            <Modal.Title>Все совпадения</Modal.Title>
        </Modal.Header>
        <Modal.Body>
            {
                elements.map((o, i) => <TransferItem key={i} item={o} />)
            }
        </Modal.Body>
    </Modal>
}

export default TransferWindow;