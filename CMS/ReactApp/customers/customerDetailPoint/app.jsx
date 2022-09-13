import React  from 'react';
import { createRoot } from 'react-dom/client';
import PointTable from "./pointTable";
const container = document.getElementById('table-point');
const root = createRoot(container);
root.render(<PointTable />);




