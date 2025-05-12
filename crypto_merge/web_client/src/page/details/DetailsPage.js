import { useState } from "react";
import { Offcanvas, Stack } from "react-bootstrap";
import Search from "./Search";
import ListOrders from "./ListOrders";

function DetailsPage() {

    document.title = 'Реквизиты';

    return <div>
        <Stack gap={5}>
            <Search />
            <ListOrders />
        </Stack>
    </div>
}

export default DetailsPage;