import { Button } from 'react-bootstrap';
import Form from 'react-bootstrap/Form';
import InputGroup from 'react-bootstrap/InputGroup';
import 'bootstrap/dist/css/bootstrap.css';
import { Stack } from 'react-bootstrap/esm';
import { useState } from 'react';
import UserWindow from './UserWindow';
import TransactionWindow from './TransactionWindow';

function MainPage() {

    document.title = "Главная";

    const Search = () => {

        const [userWindowConf, setUserWindowConf] = useState({
            show: false,
            data: null
        });
        const [transactionWindowConf, setTransactionWindowConf] = useState({
            show: false,
            data: null,
            type: 'chatId'
        });

        const [transactionSearch, setTransactionSearch] = useState('');
        const [chatIdSearch, setChatIdSearch] = useState('');
        const [requestSearch, setRequestSearch] = useState('');

        return <div>
            <Stack gap={3}>
                <div className="p-2">
                    <h1>Поиск платежа по ID</h1>
                    <InputGroup className="lg">
                        <InputGroup.Text id="inputGroup-sizing-lg">/</InputGroup.Text>
                        <Form.Control value={transactionSearch} onChange={(e) => setTransactionSearch(e.target.value)}  placeholder="ID платежа" aria-label="Large" aria-describedby="inputGroup-sizing-sm" />
                        <Button variant="outline-secondary" id="button-addon2" onClick={() => setTransactionWindowConf({
                            show: true,
                            data: transactionSearch,
                        })}>
                            Тык
                        </Button>
                    </InputGroup>
                </div>
                <div className="p-2">
                    <h1>Поиск пользователя по ID</h1>
                    <InputGroup className="lg">
                        <InputGroup.Text id="inputGroup-sizing-lg">/</InputGroup.Text>
                        <Form.Control value={chatIdSearch} onChange={(e) => setChatIdSearch(e.target.value)} placeholder="ID пользователя" aria-label="Large" aria-describedby="inputGroup-sizing-sm" />
                        <Button variant="outline-secondary" id="button-addon2" onClick={() => setUserWindowConf({
                            show: true,
                            data: chatIdSearch,
                            type: 'chatId'
                        })}>
                            Тык
                        </Button>
                    </InputGroup>
                </div>
                <div className="p-2">
                    <h1>Поиск пользователя по реквизитам</h1>
                    <InputGroup className="lg">
                        <InputGroup.Text id="inputGroup-sizing-lg">/</InputGroup.Text>
                        <Form.Control value={requestSearch} onChange={(e) => setRequestSearch(e.target.value)} placeholder="реквизиты" aria-label="Large" aria-describedby="inputGroup-sizing-sm" />
                        <Button variant="outline-secondary" id="button-addon2" onClick={() => setUserWindowConf({
                            show: true,
                            data: requestSearch,
                            type: 'request'
                        })}>
                            Тык
                        </Button>
                    </InputGroup>
                </div>
            </Stack>
            {
                transactionWindowConf.show ? 
                <TransactionWindow show={transactionWindowConf.show} onHide={() => setTransactionWindowConf({
                    show: false,
                    data: transactionWindowConf.data,
                })} request={transactionWindowConf.data} />
                : ''
            }
            {
                userWindowConf.show ? 
                <UserWindow show={userWindowConf.show} onHide={() => setUserWindowConf({
                    show: false,
                    data: userWindowConf.data,
                })} request={userWindowConf.data} type={userWindowConf.type} /> : ''
            }
        </div>
    }

    return <div>
        <Search />
    </div>
}

export default MainPage;