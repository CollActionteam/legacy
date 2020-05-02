import React, { Suspense } from 'react';
import ReactDOM from 'react-dom';
import * as serviceWorker from './serviceWorker';
import { Route, BrowserRouter as Router, Switch } from 'react-router-dom';
import Layout from './components/Layout/Layout';
import AllProviders from './providers/AllProviders';
import ScrollToTop from './components/ScrollToTop/ScrollToTop';
import RunAnalytics from './components/Analytics/RunAnalytics';
import Loader from './components/Loader/Loader';

// General pages
const HomePage = React.lazy(() => import(/* webpackChunkName: "home" */'./pages/Home/Home'));
const AboutPage = React.lazy(() => import(/* webpackChunkName: "secondary" */'./pages/About/About'));
const AdminPage = React.lazy(() => import(/* webpackChunkName: "admin" */'./pages/Admin/Admin'));
const NotFoundPage = React.lazy(() => import(/* webpackChunkName: "secondary" */'./pages/NotFound/NotFound'));
const PrivacyPolicyPage = React.lazy(() => import(/* webpackChunkName: "secondary" */'./pages/PrivacyPolicy/PrivacyPolicy'));

// Account pages
const LoginPage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/Login/Login'));
const ProfilePage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/Profile/Profile'));
const ForgotPasswordPage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/ForgotPassword/ForgotPassword'));
const RegisterUserPage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/RegisterUser/RegisterUser'));
const ResetPasswordPage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/ResetPassword/ResetPassword'));
const FinishRegistrationPage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/FinishRegistration/FinishRegistration'));

// Donation pages
const DonationReturnPage = React.lazy(() => import(/* webpackChunkName: "donation" */'./pages/Donation/DonationReturn'));
const DonationPage = React.lazy(() => import(/* webpackChunkName: "donation" */'./pages/Donation/Donation'));
const DonationThankYouPage = React.lazy(() => import(/* webpackChunkName: "donation" */"./pages/Donation/DonationThankYou"));

// Project pages
const FindPage = React.lazy(() => import(/* webpackChunkName: "project" */'./pages/projects/Find/Find'));
const ThankYouCreatePage = React.lazy(() => import(/* webpackChunkName: "project" */'./pages/projects/ThankYouCreate/ThankYouCreate'));
const ThankYouCommitPage = React.lazy(() => import(/* webpackChunkName: "project" */'./pages/projects/ThankYouCommit/ThankYouCommit'));
const ProjectDetailsPage = React.lazy(() => import(/* webpackChunkName: "project" */'./pages/projects/Detail/ProjectDetails'));
const UnsubscribeProjectPage = React.lazy(() => import(/* webpackChunkName: "project" */'./pages/projects/UnsubscribeProject/UnsubscribeProject'));
const RegistrationCompletePage = React.lazy(() => import(/* webpackChunkName: "project" */'./pages/account/RegisterUser/RegistrationComplete'));
const ProjectWidgetPage = React.lazy(() => import(/* webpackChunkName: "project" */'./pages/projects/Widget/ProjectWidget'));
const CreateProjectPage = React.lazy(() => import(/* webpackChunkName: "project-start" */'./pages/projects/Create/Create'));
const StartProjectPage = React.lazy(() => import(/* webpackChunkName: "project-start" */'./pages/projects/Start/Start'));

const routing = (
    <Router>
        <React.StrictMode>
            <Suspense fallback={<Loader />}>
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
                            <Route exact path="/donate/thankyou" component={DonationThankYouPage} />
                            <Route exact path="/projects/find" component={FindPage} />
                            <Route exact path="/projects/start" component={StartProjectPage} />
                            <Route exact path="/projects/create" component={CreateProjectPage} />
                            <Route exact path="/projects/create/thankyou/:projectId" component={ThankYouCreatePage} />
                            <Route exact path="/projects/:slug/:projectId" component={ProjectDetailsPage} />
                            <Route exact path="/projects/:slug/:projectId/thankyou" component={ThankYouCommitPage} />
                            <Route exact path="/projects/:slug/:projectId/widget" component={ProjectWidgetPage} />
                            <Route exact path="/projects/:slug/:projectId/unsubscribe-email" component={UnsubscribeProjectPage} />
                            <Route exact path="/admin/:type/:action/:id?" component={AdminPage} />
                            <Route component={NotFoundPage} />
                        </Switch>
                        <RunAnalytics />
                    </Layout>
                </AllProviders>
            </Suspense>
        </React.StrictMode>
    </Router>
);

ReactDOM.render(routing, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
