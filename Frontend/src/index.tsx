import React from 'react';
import ReactDOM from 'react-dom';
import * as serviceWorker from './serviceWorker';
import { Route, BrowserRouter as Router, Switch } from 'react-router-dom';
import Apollo from './providers/apollo';
import User from './providers/user';
import Layout from './components/Layout';
import i18n from './i18n';
import { I18nextProvider } from 'react-i18next';

// General pages
import HomePage from './pages/Home/Home';
import LoginPage from './pages/Login/Login';
import AboutPage from './pages/About/About';
import NotFoundPage from './pages/NotFound/NotFound';

// Project pages
import FindPage from './pages/projects/Find/Find';
import StartProjectPage from './pages/projects/Start/Start';
import CreateProjectPage from './pages/projects/Create/Create';
import ThankYouCreatePage from './pages/projects/ThankYouCreate/ThankYouCreate';
import ThankYouCommitPage from './pages/projects/ThankYouCommit/ThankYouCommit';
import ProjectDetailsPage from './pages/projects/Detail/ProjectDetails';
import ProfilePage from './pages/Profile/Profile';
import AdminPage from './pages/Admin/Admin';
import DonationPage from './pages/Donation/Donation';
import ForgotPasswordPage from './pages/ForgotPassword/ForgotPassword';
import RegisterUserPage from './pages/RegisterUser/RegisterUser';
import ResetPasswordPage from './pages/ResetPassword/ResetPassword';
import PrivacyPolicyPage from './pages/PrivacyPolicy/PrivacyPolicy';

const routing = (
    <I18nextProvider i18n={i18n}>
        <Router>
            <Apollo>
                <User>
                    <Switch>
                        <Route path="/admin" component={AdminPage} />
                        <Layout>
                            <Route exact path="/" component={HomePage} />
                            <Route path="/login" component={LoginPage} />
                            <Route path="/forgot-password" component={ForgotPasswordPage} />
                            <Route path="/reset-password" component={ResetPasswordPage} />
                            <Route path="/register-user" component={RegisterUserPage} />
                            <Route path="/about" component={AboutPage} />
                            <Route path="/profile" component={ProfilePage} />
                            <Route path="/donate" component={DonationPage} />
                            <Route path="/privacy-policy" component={PrivacyPolicyPage} />
                            <Route path="/projects/find" component={FindPage} />
                            <Route path="/projects/start" component={StartProjectPage} />
                            <Route path="/projects/create" component={CreateProjectPage} />
                            <Route path="/projects/thank-you-create" component={ThankYouCreatePage} />
                            <Route path="/projects/:slug/:projectId" render={routeProps => <ProjectDetailsPage {...routeProps} />} />
                            <Route path="/projects/:slug/:projectId/thankyou" render={routeProps => <ThankYouCommitPage {...routeProps} />} />
                            <Route component={NotFoundPage} />
                        </Layout>
                    </Switch>
                </User>
            </Apollo>
        </Router>
    </I18nextProvider>
);

ReactDOM.render(routing, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
