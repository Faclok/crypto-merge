import { useEffect, useState } from "react";
import { Button, Col, Form, InputGroup, Modal, Row } from "react-bootstrap";
import { SearchTransaction } from "../../api/main";

function TransactionWindow({ show, onHide, request }){

    const [transactions, setTransactions] = useState([]);
    const [currentTransaction, setCurrentTransaction] = useState();

    useEffect(() => {

        async function DidMount(){

            const response = await SearchTransaction(request);

            setTransactions(response ?? []);
        }

        DidMount();
    }, []);

    if(transactions.length <= 0)
        return <Modal show={show} onHide={onHide} size="sm">
        <Modal.Header closeButton>
            <Modal.Title>Упс...</Modal.Title>
        </Modal.Header>
        <Modal.Body>
            <p>Поиск ничего не нашел</p>
        </Modal.Body>
    </Modal>

    if(!currentTransaction)
        return <Modal show={show} onHide={onHide}
    size="lg"
    aria-labelledby="contained-modal-title-vcenter"
    centered>
        <Modal.Header closeButton>
            <Modal.Title>Все совпадения: {transactions.length}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
            {
                transactions.map(o =>  <InputGroup key={o.key}>
                    <InputGroup.Text>{new Date(o.dateTimeUTC).toJSON().replace('T', ' ').replace('Z', ' ')}</InputGroup.Text>
                    <Form.Control value={o.status}/>
                    <Button onClick={() => setCurrentTransaction(o)}>Открыть</Button>
                </InputGroup>)
            }
        </Modal.Body>
        </Modal>

    return <Modal show={show} onHide={onHide}
        size="lg"
        aria-labelledby="contained-modal-title-vcenter"
        centered>
        <Modal.Header closeButton>
            <Modal.Title>Дата: {currentTransaction.dateTimeUTC}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
            <Form>
                <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                    <Form.Label column sm="3">
                        Сообщение:
                    </Form.Label>
                    <Col sm="9">
                        <Form.Control plaintext readOnly value={currentTransaction.message} />
                    </Col>
                </Form.Group>
                <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                    <Form.Label column sm="3">
                        Сумма:
                    </Form.Label>
                    <Col sm="9">
                        <Form.Control plaintext readOnly value={currentTransaction.count} />
                    </Col>
                </Form.Group>
                <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                    <Form.Label column sm="3">
                        Тип:
                    </Form.Label>
                    <Col sm="9">
                        <Form.Control plaintext readOnly value={currentTransaction.type} />
                    </Col>
                </Form.Group>
                <Form.Group as={Row} className="mb-3" controlId="formPlaintextEmail">
                    <Form.Label column sm="3">
                        Статус:
                    </Form.Label>
                    <Col sm="9">
                        <Form.Control plaintext readOnly value={currentTransaction.status} />
                    </Col>
                </Form.Group>
            </Form>
        </Modal.Body>
        <Modal.Footer>
            <Button>Сохранить</Button>
        </Modal.Footer>
    </Modal>
}

export default TransactionWindow;