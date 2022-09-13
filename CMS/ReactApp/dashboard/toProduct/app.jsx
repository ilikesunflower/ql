import React  from 'react';
import { createRoot } from 'react-dom/client';
import MainApp from "./MainView";
const container = document.getElementById('to-product-app');
const root = createRoot(container);
root.render(
    <MainApp  />
);




