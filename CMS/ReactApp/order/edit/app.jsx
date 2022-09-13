import React  from 'react';
import { createRoot } from 'react-dom/client';
import MainApp from "./components/MainView";
const container = document.getElementById('product_app');
const root = createRoot(container);
root.render(
    <MainApp id={window.id} />
);




