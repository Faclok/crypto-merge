import { Stack } from "react-bootstrap/esm";
import TransferCommit from "./TransferCommit";
import TransferSearch from "./TransferSearch";

function TransferPage() {

    document.title = "Переводы";

    return <div>
        <Stack gap={5}>
            <TransferCommit />
            <TransferSearch />
        </Stack>
    </div>
}

export default TransferPage;