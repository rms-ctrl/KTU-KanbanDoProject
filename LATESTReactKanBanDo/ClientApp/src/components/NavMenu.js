import React, { useState } from 'react';
import {
    Collapse,
    Navbar,
    NavbarBrand,
    NavbarToggler,
    UncontrolledDropdown,
    DropdownToggle,
    DropdownMenu,
    DropdownItem
} from 'reactstrap';
import { Link, useNavigate } from 'react-router-dom';
import './NavMenu.css';
import avatar from '../pictures/avatar.png';


const NavMenu = () => {
    const [collapsed, setCollapsed] = useState(true);
    const navigate = useNavigate();

    const toggleNavbar = () => {
        setCollapsed(!collapsed);
    };

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    return (
        <header>
            <Navbar style={navbarStyle} className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
                <NavbarBrand style={navbarBrandStyle} tag={Link} to="/">Your personal Kanban Board made easy</NavbarBrand>
                <NavbarToggler onClick={toggleNavbar} className="mr-2" />
                <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
                    <ul className="navbar-nav flex-grow">
                        <UncontrolledDropdown nav inNavbar>
                            <DropdownToggle nav caret>
                                <img src={avatar} className="navbar-avatar" alt="Avatar" style={{ width: '50px', height: '50px' }} />
                            </DropdownToggle>
                            <DropdownMenu right>
                                <DropdownItem tag={Link} to="/">Home</DropdownItem>
                                <DropdownItem tag={Link} to="/login">Login</DropdownItem>
                                <DropdownItem tag={Link} to="/register">Register</DropdownItem>
                                <DropdownItem divider />
                                <DropdownItem onClick={handleLogout}>Logout</DropdownItem>
                            </DropdownMenu>
                        </UncontrolledDropdown>
                    </ul>
                </Collapse>
            </Navbar>
        </header>
    );
};

const navbarStyle = {
    backgroundColor: '#000f',
    boxShadow: 'none',
    borderBottom: '0',
    padding: '0.5rem 1rem',
};

const navLinkStyle = {
    color: '#FFFFFF',
}

const navbarBrandStyle = {
    color: '#FFFFFF',
    cursor: 'pointer',
    fontWeight: 'bold',
    fontStyle: 'italic',
};

export default NavMenu;
