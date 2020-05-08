import React, { Suspense } from 'react';
import ReactDOM from 'react-dom';
import * as serviceWorker from './serviceWorker';
import { Route, BrowserRouter as Router, Switch, Redirect } from 'react-router-dom';
import Layout from './components/Layout/Layout';
import AllProviders from './providers/AllProviders';
import ScrollToTop from './components/ScrollToTop/ScrollToTop';
import RunAnalytics from './components/Analytics/RunAnalytics';
import Loader from './components/Loader/Loader';

// General pages
const HomePage = React.lazy(() => import(/* webpackChunkName: "home" */'./pages/Home/Home'));
const AdminPage = React.lazy(() => import(/* webpackChunkName: "admin" */'./pages/Admin/Admin'));
const AboutPage = React.lazy(() => import(/* webpackChunkName: "secondary" */'./pages/About/About'));
const NotFoundPage = React.lazy(() => import(/* webpackChunkName: "secondary" */'./pages/NotFound/NotFound'));
const PrivacyPolicyPage = React.lazy(() => import(/* webpackChunkName: "secondary" */'./pages/PrivacyPolicy/PrivacyPolicy'));

// Account pages
const LoginPage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/Login/Login'));
const ProfilePage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/Profile/Profile'));
const ForgotPasswordPage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/ForgotPassword/ForgotPassword'));
const RegisterUserPage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/RegisterUser/RegisterUser'));
const ResetPasswordPage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/ResetPassword/ResetPassword'));
const FinishRegistrationPage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/FinishRegistration/FinishRegistration'));
const RegistrationCompletePage = React.lazy(() => import(/* webpackChunkName: "account" */'./pages/account/RegisterUser/RegistrationComplete'));

// Donation pages
const DonationReturnPage = React.lazy(() => import(/* webpackChunkName: "donation" */'./pages/Donation/DonationReturn'));
const DonationPage = React.lazy(() => import(/* webpackChunkName: "donation" */'./pages/Donation/Donation'));
const DonationThankYouPage = React.lazy(() => import(/* webpackChunkName: "donation" */"./pages/Donation/DonationThankYou"));

// Crowdaction pages
const FindPage = React.lazy(() => import(/* webpackChunkName: "crowdaction" */'./pages/crowdactions/Find/Find'));
const ThankYouCommitPage = React.lazy(() => import(/* webpackChunkName: "crowdaction" */'./pages/crowdactions/ThankYouCommit/ThankYouCommit'));
const CrowdactionDetailsPage = React.lazy(() => import(/* webpackChunkName: "crowdaction" */'./pages/crowdactions/Detail/CrowdactionDetails'));
const UnsubscribeCrowdactionPage = React.lazy(() => import(/* webpackChunkName: "crowdaction" */'./pages/crowdactions/UnsubscribeCrowdaction/UnsubscribeCrowdaction'));
const CrowdactionWidgetPage = React.lazy(() => import(/* webpackChunkName: "crowdaction" */'./pages/crowdactions/Widget/CrowdactionWidget'));

// Crowdaction start pages
const CreateCrowdactionPage = React.lazy(() => import(/* webpackChunkName: "crowdaction-start" */'./pages/crowdactions/Create/Create'));
const StartCrowdactionPage = React.lazy(() => import(/* webpackChunkName: "crowdaction-start" */'./pages/crowdactions/Start/Start'));
const ThankYouCreatePage = React.lazy(() => import(/* webpackChunkName: "crowdaction-start" */'./pages/crowdactions/ThankYouCreate/ThankYouCreate'));

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
                            <Route exact path="/crowdactions/find" component={FindPage} />
                            <Route exact path="/crowdactions/start" component={StartCrowdactionPage} />
                            <Route exact path="/crowdactions/create" component={CreateCrowdactionPage} />
                            <Route exact path="/crowdactions/create/thankyou/:crowdactionId" component={ThankYouCreatePage} />
                            <Route exact path="/crowdactions/:slug/:crowdactionId" component={CrowdactionDetailsPage} />
                            <Route exact path="/crowdactions/:slug/:crowdactionId/thankyou" component={ThankYouCommitPage} />
                            <Route exact path="/crowdactions/:slug/:crowdactionId/widget" component={CrowdactionWidgetPage} />
                            <Route exact path="/crowdactions/:slug/:crowdactionId/unsubscribe-email" component={UnsubscribeCrowdactionPage} />
                            <Route exact path="/admin/:type/:action/:id?" component={AdminPage} />

                            {/* Redirects from old site to new site */}
                            <Redirect exact from="/projects/:slug/:crowdactionId/details" to="/crowdactions/:slug/:crowdactionId" />
                            <Redirect exact from="/projects/embed/:crowdactionId" to="/crowdactions/_/:crowdactionId/widget" />
                            <Redirect exact from="/Donation/Donate" to="/donate" />
                            <Redirect exact from="/account/FinishRegistration" to={{
                                pathname: "/account/finish-registration",
                                search: `?email=${new URLSearchParams(window.location.search).get("email")}&code=${new URLSearchParams(window.location.search).get("code")}`
                            }} />

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
