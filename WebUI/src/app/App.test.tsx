// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { render, screen } from '@testing-library/react';
import SustainabilityApp from './SustainabilityApp';

test('renders learn react link', () => {
    render(<SustainabilityApp />);
    const linkElement = screen.getByText(/learn react/i);
    expect(linkElement).toBeInTheDocument();
});
