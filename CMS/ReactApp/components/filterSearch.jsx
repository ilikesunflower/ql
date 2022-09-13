import React from 'react';

export const filterOptions = (options, filter) => {
    if (!filter) {
        return options;
    }
    const re = new RegExp(filter, "i");
    return options.filter(({ label }) => label && label.match(re));
};

