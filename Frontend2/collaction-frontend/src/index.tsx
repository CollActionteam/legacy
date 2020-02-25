import React from 'react';
import ReactDOM from 'react-dom';
import * as serviceWorker from './serviceWorker';
import { Route, BrowserRouter as Router, Switch } from 'react-router-dom';
import Apollo from './providers/apollo';
import Layout from './components/Layout';

// Pages
import HomePage from './pages/Home/Home';
import FindPage from './pages/projects/Find/Find';
import NotFoundPage from './pages/NotFound/NotFound';
import StartProjectPage from './pages/projects/Start/Start';
import CreateProjectPage from './pages/projects/Create/Create';
import ThankYouPage from './pages/projects/ThankYouCreate/ThankYouCreate';

const routing = (
    <Router>
        <Apollo>
            <Layout>
                <Switch>
                    <Route exact path="/" component={HomePage} />
                    <Route path="/projects/find" component={FindPage} />
                    <Route path="/projects/start" component={StartProjectPage} />
                    <Route path="/projects/create" component={CreateProjectPage} />
                    <Route path="/projects/thank-you" component={ThankYouPage} />
                    <Route component={NotFoundPage} />
                </Switch>
            </Layout>
        </Apollo>
    </Router>
);

ReactDOM.render(routing, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
