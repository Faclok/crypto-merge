import { useState } from "react";
import { Button, Form, Modal } from "react-bootstrap";
import { postSetBalance } from "../../api/main";


function WalletEditWindow({ show, onHide, balance, chatId, setBalance }) {

    const [valueEdit, setValueEdit] = useState('');
    const [comment, setComment] = useState('');
    const [typeEdit, setTypeEdit] = useState(1);
    const [validated, setValidated] = useState(false);

    const handleSubmit = async(event) => {
      const form = event.currentTarget;
      event.preventDefault();

      if (form.checkValidity() === false) {
        event.stopPropagation();
      }else {
        await postSetBalance(chatId, typeEdit, valueEdit, comment);
        setBalance(typeEdit == 1 ? +balance + +valueEdit : valueEdit);
        onHide();
      }
  
      setValidated(true);
    };

    return <Modal
        show={show}
        onHide={onHide}
        size="lg"
        aria-labelledby="contained-modal-title-vcenter"
        centered>
        <Form noValidate validated={validated} onSubmit={handleSubmit}>
            <Modal.Header closeButton>
                <Modal.Title>Изменение баланса</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form.Group>
                    <Form.Label>Тип изменения</Form.Label>
                    <Form.Select value={typeEdit} onChange={(e) => setTypeEdit(e.target.value)}>
                        <option value={1}>На указанную сумму</option>
                        <option value={2}>Новый баланс</option>
                    </Form.Select>
                </Form.Group>
                <Form.Group>
                    <Form.Label>Сумма</Form.Label>
                    <Form.Control required value={valueEdit} type="number" onChange={(e) => setValueEdit(e.target.value)} />
                </Form.Group>
                <Form.Group>
                    <Form.Label>Комментарий</Form.Label>
                    <Form.Control required value={comment} type="text" onChange={(e) => setComment(e.target.value)} />
                </Form.Group>
            </Modal.Body>
            <Modal.Footer>
                <Button type="submit">Сохранить</Button>
            </Modal.Footer>
        </Form>
    </Modal>
}

export default WalletEditWindow;