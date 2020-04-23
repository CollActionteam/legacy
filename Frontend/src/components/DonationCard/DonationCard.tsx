import React, { useState } from "react";
import { FormGroup, FormControl, TextField, Grid, Dialog, DialogTitle, DialogContent, DialogActions, makeStyles } from "@material-ui/core";
import { useFormik, Form, FormikContext } from "formik";
import { useUser } from "../../providers/UserProvider";
import * as Yup from "yup";
import { Alert } from "../Alert/Alert";
import styles from "./DonationCard.module.scss";
import bankCard from "../../assets/bank-card.png";
import iDealLogo from "../../assets/i-deal-logo.png";
import { useMutation, gql } from "@apollo/client";
import { loadStripe } from '@stripe/stripe-js';
import { GET_USER } from "../../providers/UserProvider";
import {
  Elements,
  useStripe,
  IdealBankElement,
  IbanElement,
  useElements,
} from '@stripe/react-stripe-js';
import { Button } from "../Button/Button";
import {Link, useHistory} from "react-router-dom";
import { useConsent, Consent } from "../../providers/CookieConsentProvider";
import { useSettings } from "../../providers/SettingsProvider";

type DonationValues = {
    email: string;
    name: string;
    type: string;
    amount: number;
    recurring: boolean;
};

const INITIALIZE_CREDIT_CARD_CHECKOUT = gql`
    mutation InitializeCreditCardCheckout($checkout: CreditCardCheckoutInputGraph!) {
        donation {
            sourceId: initializeCreditCardCheckout(checkout: $checkout)
        }
    }
`;

const INITIALIZE_IDEAL_CHECKOUT = gql`
    mutation InitializeIDealCheckout($checkout: IDealCheckoutInputGraph!) {
        donation {
            sourceId: initializeIDealCheckout(checkout: $checkout)
        }
    }
`;

const INITIALIZE_SEPA_DIRECT = gql`
    mutation InitializeSEPADirect($checkout: SepaDirectCheckoutInputGraph!) {
        donation {
            sourceId: initializeSepaDirect(checkout: $checkout)
        }
    }
`;

const dialogStyles = {
    dialogPaper: {
        minHeight: '390px'
    }
};
const useDialogStyles = makeStyles(dialogStyles);

const InnerDonationCard = () => {
    const user = useUser();
    const [ error, setError ] = useState<string | null>(null);
    const [ bankingPopupOpen, setBankingPopupOpen ] = useState(false);
    const stripe = useStripe();
    const dialogClasses = useDialogStyles();
    const elements = useElements();
    const history = useHistory();
    const amounts = [ 3, 5, 10, 20, 30, 50, 100 ];

    const [ initializeCreditCardCheckout ] = useMutation(
        INITIALIZE_CREDIT_CARD_CHECKOUT
    );

    const [ initializeIdealCheckout ] = useMutation(
        INITIALIZE_IDEAL_CHECKOUT
    );

    const [ initializeSepaDirect ] = useMutation(
        INITIALIZE_SEPA_DIRECT,
        {
            refetchQueries: [{
                query: GET_USER // Ensure the donation appears in the profile page after this call succeeds
            }]
        }
    );

    const payCreditcard = async (values: DonationValues) => {
        const creditCardResult = await initializeCreditCardCheckout({
            variables: {
                checkout: {
                    amount: values.amount,
                    currency: 'eur',
                    name: values.name,
                    email: values.email,
                    recurring: values.recurring,
                    successUrl: window.location.origin + '/donate/thankyou',
                    cancelUrl: window.location.origin + '/donate'
                }
            },
        });
        if (creditCardResult.errors) {
            const err = creditCardResult.errors.map(e => e.message).join(", ");
            setError(err);
            console.log(err);
            return;
        }
        await stripe!.redirectToCheckout({ sessionId: creditCardResult.data.donation.sourceId });
    };

    const payIban = async (values: DonationValues) => {
        const ibanElement = elements?.getElement(IbanElement);
        if (!ibanElement) {
            console.error(`Unable to start the SEPA direct transaction: IBAN element not found`);
            setError("We're unable to start your SEPA direct donation, something is wrong, please contact collactionteam@gmail.com with your issue");
            return;
        }
        let response = await stripe!.createSource(ibanElement, {
            type: "sepa_debit",
            currency: "eur",
            owner: {
                name: values.name,
                email: values.email
            },
            mandate: {
                notification_method: 'email' // Automatically send a mandate notification email to your customer once the source is charged
            }
        });
        if (response.error) {
            console.error(`Unable to start the SEPA direct transaction: ${response.error.message}`);
            setError("We're unable to start your SEPA direct donation, something is wrong, please contact collactionteam@gmail.com with your issue");
            return;
        }

        const sourceId = response!.source!.id;
        const initializeResult = await initializeSepaDirect({
            variables: {
                checkout: {
                    sourceId: sourceId,
                    name: values.name,
                    email: values.email,
                    amount: values.amount
                }
            }
        });
        if (initializeResult.errors) {
            console.error(`Unable to start the SEPA direct transaction: ${initializeResult.errors.map(e => e.message).join(", ")}`);
            setError("We're unable to start your SEPA direct donation, something is wrong, please contact collactionteam@gmail.com with your issue");
            return;
        }
        history.push('/donate/thankyou');
    };

    const payIDeal = async (values: DonationValues) => {
        const response = await stripe!.createSource({
            type: "ideal",
            amount: values.amount * 100,
            currency: "eur",
            statement_descriptor: "Donation CollAction",
            owner: {
                name: values.name,
                email: values.email
            },
            redirect: {
                return_url: window.location.origin + "/donate/return"
            }
        });
        if (response.error) {
            console.error(`Unable to start the iDeal transaction: ${response.error.message}`);
            setError("We're unable to start your iDeal donation, something is wrong, please contact collactionteam@gmail.com with your issue");
            return;
        }
        const sourceId = response!.source!.id;
        const redirectUrl = response!.source!.redirect!.url;
        const initializeResult = await initializeIdealCheckout({
            variables: {
                checkout: {
                    sourceId: sourceId,
                    name: values.name,
                    email: values.email
                }
            }
        });
        if (initializeResult.errors) {
            console.error(`Unable to start the iDeal transaction: ${initializeResult.errors.map(e => e.message).join(", ")}`);
            setError("We're unable to start your iDeal donation, something is wrong, please contact collactionteam@gmail.com with your issue");
            return;
        }
        window.location.href = redirectUrl;
    };

    const formik = useFormik({
        initialValues: {
            name: user?.fullName ?? "",
            email: user?.email ?? "",
            recurring: false,
            amount: 5,
            type: ""
        },
        validationSchema: Yup.object({
            email: Yup.string().required("Please enter your E-Mail address").email("Please enter a valid E-Mail address"),
            name: Yup.string().required("Please provide your name"),
            recurring: Yup.boolean(),
            amount: Yup.number().min(1, "Your donation amount must be positive").typeError("You must specify a number as donation amount"),
            type: Yup.string()
        }),
        onSubmit: async (values) => {
            if (values.type === "popup") {
                setBankingPopupOpen(true);
            } else if (values.type === "credit") {
                await payCreditcard(values);
            } else if (values.type === "debit") {
                if (values.recurring) {
                    await payIban(values);
                } else {
                    await payIDeal(values);
                }
            }
        }
    });

    const amountCheckbox = (amount: number) =>
        <Grid item key={amount} xs={6} sm={3} className={styles.paymentToggle}>
            <input id={`donation-amount-${amount}`} type="radio" name="amount" value={amount} checked={formik.values.amount === amount} onChange={() => formik.setFieldValue("amount", amount)} />
            <label htmlFor={`donation-amount-${amount}`}>&euro;{ amount }</label>
        </Grid>;
    
    const bankingDialog = () => {
        const IBAN_OPTIONS = {
            supportedCountries: ['SEPA']
        };
        return <Dialog fullWidth classes={{ paper: dialogClasses.dialogPaper }} open={bankingPopupOpen} onClose={() => { setBankingPopupOpen(false); }}>
            <DialogTitle>
                Donate
            </DialogTitle>
            <DialogContent dividers>
                { formik.values.recurring ?
                    <React.Fragment>
                        <Alert type="warning" icon="exclamation-circle" text="By providing your IBAN and confirming this payment, you are authorizing Stichting CollAction and Stripe, our payment service provider, to send instructions to your bank to debit your account and your bank to debit your account in accordance with those instructions. You are entitled to a refund from your bank under the terms and conditions of your agreement with your bank. A refund must be claimed within 8 weeks starting from the date on which your account was debited." />
                        <IbanElement options={IBAN_OPTIONS} />
                    </React.Fragment> :
                    <IdealBankElement />
                }
            </DialogContent>
            <DialogActions>
                <Button onClick={() => { formik.setFieldValue('type', 'debit'); formik.submitForm() }}>Donate</Button>
                <Button onClick={() => setBankingPopupOpen(false)}>Close</Button>
            </DialogActions>
        </Dialog>
    };

    return <div className={styles.card}>
        <Alert type="error" text={error} />
        <h2>Help us reach our mission by donating!</h2>
        CollAction aims to move millions of people to act for good. We're a small team of passionate volunteers and we keep cost super low - but some costs are involved in maintaining and growing the platform. With your support we can grow this crowdacting movement and safeguard our independence. Many thanks for your help!
        <FormikContext.Provider value={formik}>
            <Form onSubmit={formik.handleSubmit} className={styles.donationForm}>
                <FormGroup>
                    { user?.fullName ?
                        null :
                        <FormControl fullWidth>
                            <TextField
                                name="name"
                                label="Name"
                                type="text"
                                fullWidth
                                { ...formik.getFieldProps('name') }
                            />
                            { formik.touched.name ? <Alert type="error" text={formik.errors.name} /> : null }
                        </FormControl>
                    }
                    { user?.email ?
                        null :
                        <FormControl fullWidth>
                            <TextField
                                name="email"
                                label="E-Mail"
                                type="text"
                                fullWidth
                                { ...formik.getFieldProps('email') }
                            />
                            { formik.touched.email ? <Alert type="error" text={formik.errors.email} /> : null }
                        </FormControl>
                    }
                    <br />
                    <Grid container className={styles.paymentSelectionOptions}>
                        <Grid item key="one-off" xs={12} sm={6} className={styles.paymentToggle}>
                            <input id="one-off-donation-button" type="radio" name="period" value="one-off" checked={formik.values.recurring === false} onChange={() => formik.setFieldValue("recurring", false)} />
                            <label htmlFor="one-off-donation-button">One-off</label>
                        </Grid>
                        <Grid item key="periodic" xs={12} sm={6} className={styles.paymentToggle}>
                            <input id="periodic-donation-button" type="radio" name="period" value="periodic" checked={formik.values.recurring} onChange={() => formik.setFieldValue("recurring", true)} />
                            <label htmlFor="periodic-donation-button">Periodic</label>
                        </Grid>
                    </Grid>
                    <Grid container spacing={4} className={styles.paymentSelectionOptions}>
                        { amounts.map((amount: number) => amountCheckbox(amount)) }
                        <Grid item key="custom" xs={6} sm={3} className={styles.paymentToggle}>
                            <input name="customAmount" type="text" onChange={(e) => formik.setFieldValue('amount', parseInt(e.target.value))} placeholder="Other" />
                        </Grid>
                        <Alert type="error" text={formik.errors.amount} />
                    </Grid>
                    <Grid container className={styles.paymentOptions}>
                        <Grid item xs={12}>
                            <Button type="button" className={styles.paymentButton} onClick={async () => { formik.setFieldValue('type', 'popup'); formik.submitForm(); }}>
                                <img src={iDealLogo} alt="iDeal" />
                                &nbsp;Debit (iDeal / SEPA Direct)
                            </Button>
                        </Grid>
                        <Grid item xs={12}>
                            <Button type="button" className={styles.paymentButton} onClick={() => { formik.setFieldValue('type', 'credit'); formik.submitForm(); }}>
                                <img src={bankCard} alt="Creditcard" />
                                &nbsp;Creditcard
                            </Button>
                        </Grid>
                    </Grid>
                </FormGroup>
                { bankingDialog() }
            </Form>
        </FormikContext.Provider>
    </div>;
}

export default () => {
    const { consent } = useConsent();
    const { stripePublicKey } = useSettings();

    if (!consent.includes(Consent.Stripe)) {
        return <Alert type="error">
            You can't donate because you haven't given consent for loading the Stripe donation integration.
            Visit the <Link to="/privacy-policy">privacy policy</Link> to manage your consent.
        </Alert>;
    } else {
        const stripePromise = loadStripe(stripePublicKey);
        return <Elements stripe={stripePromise}>
            <InnerDonationCard />
        </Elements>;
    }
};