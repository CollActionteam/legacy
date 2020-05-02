import React from 'react';
import ReactDOM from 'react-dom';
import * as serviceWorker from './serviceWorker';
import { Route, BrowserRouter as Router, Switch } from 'react-router-dom';
import Layout from './components/Layout/Layout';
import AllProviders from './providers/AllProviders';

// General pages
import HomePage from './pages/Home/Home';
import LoginPage from './pages/account/Login/Login';
import AboutPage from './pages/About/About';
import AdminPage from './pages/Admin/Admin';
import NotFoundPage from './pages/NotFound/NotFound';

// Account pages
import ProfilePage from './pages/account/Profile/Profile';
import ForgotPasswordPage from './pages/account/ForgotPassword/ForgotPassword';
import RegisterUserPage from './pages/account/RegisterUser/RegisterUser';
import ResetPasswordPage from './pages/account/ResetPassword/ResetPassword';
import FinishRegistrationPage from './pages/account/FinishRegistration/FinishRegistration';

// Donation pages
import DonationReturnPage from './pages/Donation/DonationReturn';
import DonationPage from './pages/Donation/Donation';

// Project pages
import FindPage from './pages/projects/Find/Find';
import StartProjectPage from './pages/projects/Start/Start';
import CreateProjectPage from './pages/projects/Create/Create';
import ThankYouCreatePage from './pages/projects/ThankYouCreate/ThankYouCreate';
import ThankYouCommitPage from './pages/projects/ThankYouCommit/ThankYouCommit';
import ProjectDetailsPage from './pages/projects/Detail/ProjectDetails';
import PrivacyPolicyPage from './pages/PrivacyPolicy/PrivacyPolicy';
import UnsubscribeProjectPage from './pages/projects/UnsubscribeProject/UnsubscribeProject';
import RegistrationCompletePage from './pages/account/RegisterUser/RegistrationComplete';
import ScrollToTop from './components/ScrollToTop/ScrollToTop';
import RunAnalytics from './components/Analytics/RunAnalytics';

const routing = (
    <Router>
        <AllProviders>
            <Layout>
                <ScrollToTop />
                <Switch>
                    <Route exact path="/" component={HomePage} />
                    <Route exact path="/about" component={AboutPage} />
                    <Route exact path="/privacy-policy" component={PrivacyPolicyPage} />
                    <Route exact path="/account/login" component={LoginPage} />
                    <Route exact path="/account/forgot-password" component={ForgotPasswordPage} />
                    <Route exact path="/account/reset-password" component={ResetPasswordPage} />
                    <Route exact path="/account/register-user" component={RegisterUserPage} />
                    <Route exact path="/account/register-user/complete" component={RegistrationCompletePage} />
                    <Route exact path="/account/finish-registration" component={FinishRegistrationPage} />
                    <Route exact path="/account/profile" component={ProfilePage} />
                    <Route exact path="/donate" component={DonationPage} />
                    <Route exact path="/donate/return" component={DonationReturnPage} />
                    <Route exact path="/donate/:thankyou" component={DonationPage} />
                    <Route exact path="/projects/find" component={FindPage} />
                    <Route exact path="/projects/start" component={StartProjectPage} />
                    <Route exact path="/projects/create" component={CreateProjectPage} />
                    <Route exact path="/projects/create/thankyou/:projectId" component={ThankYouCreatePage} />
                    <Route exact path="/projects/:slug/:projectId" component={ProjectDetailsPage} />
                    <Route exact path="/projects/:slug/:projectId/thankyou" component={ThankYouCommitPage} />
                    <Route exact path="/projects/:slug/:projectId/unsubscribe-email" component={UnsubscribeProjectPage} />
                    <Route exact path="/admin/:type/:action/:id?" component={AdminPage} />
                    <Route component={NotFoundPage} />
                </Switch>
                <RunAnalytics />
            </Layout>
        </AllProviders>
    </Router>
);

ReactDOM.render(routing, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
