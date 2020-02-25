import React from 'react';
import ReactDOM from 'react-dom';
import * as serviceWorker from './serviceWorker';
import { Route, BrowserRouter as Router, Switch } from 'react-router-dom';
import Apollo from './providers/apollo';
import Layout from './components/Layout';

// General pages
import HomePage from './pages/Home/Home';
import LoginPage from './pages/Login/Login';
import AboutPage from './pages/About/About';
import NotFoundPage from './pages/NotFound/NotFound';

// Project pages
import FindPage from './pages/projects/Find/Find';
import StartProjectPage from './pages/projects/Start/Start';
import CreateProjectPage from './pages/projects/Create/Create';
import ThankYouPage from './pages/projects/ThankYouCreate/ThankYouCreate';
import ProjectDetailsPage from './pages/projects/Detail/ProjectDetails';

const routing = (
    <Router>
        <Apollo>
            <Layout>
                <Switch>
                    <Route exact path="/" component={HomePage} />
                    <Route path="/login" component={LoginPage} />
                    <Route path="/about" component={AboutPage} />
                    <Route path="/projects/find" component={FindPage} />
                    <Route path="/projects/start" component={StartProjectPage} />
                    <Route path="/projects/create" component={CreateProjectPage} />
                    <Route path="/projects/thank-you-create" component={ThankYouPage} />
                    <Route path="/projects/:slug/:projectId" render={routeProps => <ProjectDetailsPage {...routeProps} />} />
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
