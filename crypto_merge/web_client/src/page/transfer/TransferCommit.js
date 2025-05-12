import { useState } from 'react';
import { Button, InputGroup } from 'react-bootstrap';
import Form from 'react-bootstrap/Form';
import { createTransaction } from '../../api/transfer';

function TransferCommit() {

    const [isHard, setIsHard] = useState(true);

    const createAPI = async(cc, request, sum) => {

        const responseCode = await createTransaction({
            transactionIdCC: cc,
            requestProperty: request,
            count: sum
        });

        if(responseCode != 204)
            alert('Не получилось создать транзакцию');

        return responseCode;
    }

    const EasySearch = () => {

        const [cc, setCC] = useState('');
        const [request, setRequest] = useState('');
        const [sum, setSum] = useState('');
        const [validated, setValidated] = useState(false);

        const handleSubmit = async(event) => {
            const form = event.currentTarget;
            event.preventDefault();

            if (form.checkValidity() === false) {
                event.stopPropagation();
            } else {
                const status = await createAPI(cc, request, sum)

                if(status == 409)
                    alert('Заявка с таким CC уже зарегистрирована');
                setCC('');
                setRequest('');
                setSum('');
            }

            setValidated(true);
        };

        return <Form noValidate validated={validated} onSubmit={handleSubmit}>
            <Form.Group className="mb-3" controlId="exampleForm.ControlTextarea1">
                <Form.Label>ID транзакции</Form.Label>
                <Form.Control required placeholder='XXXXXXXX' value={cc} onChange={(e) => setCC(e.target.value)} />
                <Form.Control.Feedback type="invalid">Обязательно</Form.Control.Feedback>
            </Form.Group>
            <Form.Group className="mb-3" controlId="exampleForm.ControlTextarea1">
                <Form.Label>Реквизиты</Form.Label>
                <Form.Control required placeholder='XXXXXXXXXXX' value={request} onChange={(e) => setRequest(e.target.value)} />
                <Form.Control.Feedback type="invalid">Обязательно</Form.Control.Feedback>
            </Form.Group>
            <Form.Group className="mb-3" controlId="exampleForm.ControlTextarea1">
                <InputGroup className='mb-3'>
                    <InputGroup.Text>
                        Сумма
                    </InputGroup.Text>
                    <Form.Control required placeholder='XXXX.XXX' type='number' value={sum} onChange={(e) => setSum(e.target.value)} />
                    <Form.Control.Feedback type="invalid">Обязательно</Form.Control.Feedback>
                </InputGroup>
            </Form.Group>
            <Button type='submit'>Тык</Button>
        </Form>
    }

    const HardSearch = () => {

        const [text, setText] = useState('');
        const [validated, setValidated] = useState(false);

        const handleSubmit = async(event) => {
            const form = event.currentTarget;
            event.preventDefault();

            if (form.checkValidity() === false) {
                event.stopPropagation();
            } else {

                const [cc, request, sum] = text.replaceAll('-', '').replaceAll('+', '').replaceAll('\n', ' ').replaceAll('\t', ' ').split(' ').map(o => +o).filter(o => !isNaN(o) && o > 0);

                if(!cc || !request || !sum)
                    return;

                const status = await createAPI(cc, request, sum);
                if(status == 409)
                    alert('Заявка с таким CC уже зарегистрирована');
                setText('');
            }

            setValidated(true);
        };

        return <Form noValidate validated={validated} onSubmit={handleSubmit}>
            <Form.Group className="mb-3" controlId="exampleForm.ControlTextarea1">
                <Form.Label>Вставьте скопированный текст</Form.Label>
                <Form.Control required as="textarea" rows={3} value={text} onChange={(e) => setText(e.target.value)} placeholder='118338832  4276280015248922  950  10.208  + 0.408'/>
            </Form.Group>
            <Button type='submit'>Тык</Button>
        </Form>
    }

    return <div>
        <h2> <Form.Check
            type="switch"
            id="custom-switch"
            checked={isHard}
            value={isHard}
            onChange={(e) => setIsHard(e.target.checked)}
            label='Создание выкупа'
        /></h2>
        {
            isHard ? <HardSearch /> : <EasySearch />
        }
    </div>
}

export default TransferCommit;