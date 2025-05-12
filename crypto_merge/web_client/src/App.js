import { Link, Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import styles from './App.module.css';
import MainPage from './page/main/MainPage';
import DetailsPage from './page/details/DetailsPage';
import DepositPage from './page/deposit/DepositPage';
import TransferPage from './page/transfer/TransferPage';
import { TbBrandSupabase, TbLockCheck } from 'react-icons/tb';
import { PiHandDeposit } from 'react-icons/pi';
import { FaRegIdCard } from 'react-icons/fa';
import { BiTransfer } from 'react-icons/bi';
import LimitPage from './page/limit/LimitPage';
import { useEffect, useState } from 'react';
import { getCountLimits } from './api/limit';
import { getCurrency, putCurrencyRub } from './api/currency';
import { Button, Form, Modal } from 'react-bootstrap';
import { getStatus, sendCode } from './api/telegramClient';

function App() {

  const [countLimits, setCountLimits] = useState(0);
  const [currency, setCurrency] = useState();
  const [isVisibleCurrency, setIsVisibleCurrency] = useState(false);
  const [isLoginBot, setIsLoginBot] = useState();
  const [isLoginBotTwo, setIsLoginBotTwo] = useState();

  useEffect(() => {

    async function didMount() {

      setCountLimits((await getCountLimits()) ?? 0);
      setCurrency((await getCurrency('USD')))
      setIsLoginBot(await getStatus(1));
      setIsLoginBotTwo(await getStatus(2));
    }

    didMount();
  }, []);

  const CurrencyModal = () => {

    const [value, setValue] = useState(currency.rubCurrency);
    const [validated, setValidated] = useState(false);

    const handleSubmit = async (event) => {
      const form = event.currentTarget;
      event.preventDefault();

      if (form.checkValidity() === false) {
        event.stopPropagation();
      } else {
        currency.rubCurrency = value;
        await putCurrencyRub(currency.title, value);
        setIsVisibleCurrency(false);
      }

      setValidated(true);
    };

    return <Modal show={isVisibleCurrency} onHide={() => setIsVisibleCurrency(false)}
      size="lg"
      aria-labelledby="contained-modal-title-vcenter"
      centered>
      <Form noValidate validated={validated} onSubmit={handleSubmit}>
        <Modal.Header>
          <Modal.Title>{currency.title}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Group>
            <Form.Control placeholder='XX.X' value={value} onChange={(e) => setValue(e.target.value)} type='text' />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button type='submit'>Сохранить</Button>
        </Modal.Footer>
      </Form>
    </Modal>
  }

  const CodeModal = () => {

    const [code, setCode] = useState('');
    const [validated, setValidated] = useState(false);

    const handleSubmit = async (event) => {
      const form = event.currentTarget;
      event.preventDefault();

      if (form.checkValidity() === false) {
        event.stopPropagation();
      } else {
        await sendCode(code, isLoginBot ? 2: 1);
        window.location.reload();
      }

      setValidated(true);
    };

    return <Modal show={true} onHide={() => {}}
      size="lg"
      aria-labelledby="contained-modal-title-vcenter"
      centered>
      <Form noValidate validated={validated} onSubmit={handleSubmit}>
        <Modal.Header>
          <Modal.Title>Код для бота №{isLoginBot ? 2 : 1}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Group>
            <Form.Label>Код для бота</Form.Label>
            <Form.Control placeholder='XXXXX' value={code} onChange={(e) => setCode(e.target.value)} type='number' />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button type='submit'>Отправить</Button>
        </Modal.Footer>
      </Form>
    </Modal>
  }

  if (isLoginBot !== true && isLoginBot !== false)
    return <Modal show={true} onHide={() => {}}
      size="lg"
      aria-labelledby="contained-modal-title-vcenter"
      centered>
      <Form>
        <Modal.Header>
          <Modal.Title>Загрузка</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <h3>Ожидайте...</h3>
        </Modal.Body>
      </Form>
    </Modal>

  if(isLoginBot === false || isLoginBotTwo === false)
    return <CodeModal/>;

  return <div style={{ padding: 30 }}>
    <Router>
      <ul className={styles.containerUl}>
        <li className={styles.elementLi}>
          <TbBrandSupabase className={styles.elementIcon} />
          <Link className={styles.elementLink} to='/'>Основное</Link>
        </li>
        <li className={styles.elementLi}>
          <PiHandDeposit className={styles.elementIcon} />
          <Link className={styles.elementLink} to='/deposits'>Пополнения</Link>
        </li>
        <li className={styles.elementLi}>
          <FaRegIdCard className={styles.elementIcon} />
          <Link className={styles.elementLink} to='/details'>Реквизиты</Link>
        </li>
        <li className={styles.elementLi}>
          <BiTransfer className={styles.elementIcon} />
          <Link className={styles.elementLink} to='/transfers'>Переводы</Link>
        </li>
        <li className={styles.elementLi}>
          <TbLockCheck className={styles.elementIcon} />
          <Link className={styles.elementLink} to='/limits'>Лимиты {countLimits == 0 ? '' : `(${countLimits})`}</Link>
        </li>
        {
          currency ?
            <li className={styles.elementLi}>
              <Button variant='success' onClick={() => setIsVisibleCurrency(true)}>1{currency.charCurrency} = {currency.rubCurrency}₽</Button>
            </li> : ''
        }
      </ul>
      <Routes>
        <Route path='/*' element={<MainPage />} />
        <Route path='/details' element={<DetailsPage />} />
        <Route path='/deposits' element={<DepositPage />} />
        <Route path='/transfers' element={<TransferPage />} />
        <Route path='/limits' element={<LimitPage title={`Лимиты ${countLimits == 0 ? '' : `(${countLimits})`}`} />} />
      </Routes>
    </Router>

    {
      isVisibleCurrency ?
        <CurrencyModal /> : ''
    }
  </div>
}

export default App;
