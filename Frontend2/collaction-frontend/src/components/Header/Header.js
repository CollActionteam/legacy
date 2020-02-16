import React from 'react';
import { NavLink } from 'react-router-dom';

const Header = () => {
    return (
        <header>
            <NavLink to="/">Home</NavLink>
            <NavLink to="/find">Find project</NavLink>
            <NavLink to="/start">Start project</NavLink>
        </header>
    )
};

export default Header;