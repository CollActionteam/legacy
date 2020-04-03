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
import DonationThankYouPage from './pages/Donation/DonationThankYou';
import DonationReturnPage from './pages/Donation/DonationReturn';

const routing = (
    <I18nextProvider i18n={i18n}>
        <Router>
            <Apollo>
                <User>
                    <Layout>
                        <Switch>
                            <Route exact path="/" component={HomePage} />
                            <Route exact path="/login" component={LoginPage} />
                            <Route exact path="/forgot-password" component={ForgotPasswordPage} />
                            <Route exact path="/reset-password" component={ResetPasswordPage} />
                            <Route exact path="/register-user" component={RegisterUserPage} />
                            <Route exact path="/about" component={AboutPage} />
                            <Route exact path="/profile" component={ProfilePage} />
                            <Route exact path="/donate" component={DonationPage} />
                            <Route exact path="/donate/return" component={DonationReturnPage} />
                            <Route exact path="/donate/thankyou" component={DonationThankYouPage} />
                            <Route exact path="/privacy-policy" component={PrivacyPolicyPage} />
                            <Route exact path="/projects/find" component={FindPage} />
                            <Route exact path="/projects/start" component={StartProjectPage} />
                            <Route exact path="/projects/create" component={CreateProjectPage} />
                            <Route exact path="/projects/thank-you-create" component={ThankYouCreatePage} />
                            <Route exact path="/projects/:slug/:projectId" component={ProjectDetailsPage} />
                            <Route exact path="/projects/:slug/:projectId/thankyou" component={ThankYouCommitPage} />
                            <Route exact path="/admin/:type/:action/:id?" component={AdminPage} />
                            <Route component={NotFoundPage} />
                        </Switch>
                    </Layout>
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
