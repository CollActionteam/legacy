import React from 'react';
import ReactDOM from 'react-dom';
import * as serviceWorker from './serviceWorker';
import { Route, BrowserRouter as Router, Switch } from 'react-router-dom';

import Apollo from './providers/apollo';
import App from './App';
import FindPage from './pages/Find/FindPage';
import NotFoundPage from './pages/404/404';
import Layout from './components/Layout/Layout';

const routing = (
    <Apollo>
        <Router>
            <Layout>
                <Switch>
                    <Route exact path="/" component={App} />
                    <Route path="/find" component={FindPage} />
                    <Route component={NotFoundPage} />
                </Switch>
            </Layout>
        </Router>
    </Apollo>
);

ReactDOM.render(routing, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
