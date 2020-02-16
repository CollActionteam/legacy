import React from 'react';
import Header from '../Header/Header';
import Helmet from 'react-helmet';

const Layout = ({ children }) => {
    return (
        <React.Fragment>
            <Helmet>
                <title>CollAction</title>
                <meta name="description" content="CollAction" />
            </Helmet>
            <Header />
            <main>
                {children}
            </main>    
            <footer></footer>
        </React.Fragment>
    );
};

export default Layout;