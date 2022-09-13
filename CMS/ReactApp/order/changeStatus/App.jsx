import React  from 'react';
import { createRoot } from 'react-dom/client';
import MainView from "./MainView";
const container = document.getElementById('groupStatusBtn');
const root = createRoot(container);
root.render(
    <MainView />
);