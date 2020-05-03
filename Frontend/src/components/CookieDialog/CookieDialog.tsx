import React, { useState } from "react";
import styles from "./CookieDialog.module.scss";
import { Link, useLocation } from "react-router-dom";
import { Button } from "../Button/Button";
import { Container, FormControlLabel, Checkbox, FormGroup, Grid } from "@material-ui/core";
import { AllowedConsents, Consent, ConsentDescription, useConsent } from "../../providers/CookieConsentProvider";
import { useFormik, Form, FormikProvider } from "formik";
import { Alert } from "../Alert/Alert";

const initialConsentState = AllowedConsents.map(c => c === Consent.Basics ? { key: c, value: true } : { key: c, value: false })
                                           .reduce((map: { [id: string] : boolean }, { key, value }) => {
                                               map[key] = value;
                                               return map;
                                           }, {});

export default () => {
    const { consent, setConsent, setAllowAllConsent } = useConsent();
    const [ showMoreOptions, setShowMoreOptions ] = useState(false);
    const [ info, setInfo ] = useState<string | null>(null);
    const location = useLocation();
    const alwaysShow = location.pathname === "/privacy-policy";
    const formik = useFormik({
        initialValues: initialConsentState,
        onSubmit: (values) => {
            const selectedConsents = 
                Object.entries(values)
                      .filter(([_k, v]) => v)
                      .map(([k, _v]) => parseInt(k) as Consent);
            setConsent(selectedConsents);
            setInfo("You've given consent. This dialog will remain open in case you want to further update your choices.")
            formik.setSubmitting(false);
        }
    });

    if (consent.length > 0 && !alwaysShow) {
        return null;
    }

    return <div className={styles.cookieDialog}>
        <Container>
            <Grid container>
                <Grid item xs={12}>
                    <Alert type="info" text={info} />
                    Thank you for visiting our website!
                    Please note that our website uses cookies to analyze and improve the performance of our website and to make social media integration possible.
                    For more information on the use of cookies and privacy related matters, please see our <Link to="/privacy-policy">Privacy and Cookies Policy</Link>.
                    By clicking "Accept", you consent to the use of cookies, analytics and social-media integration.
                    Click on "More options" to customize your choice.
                    If you want to change your choice later, please visit our <Link to="/privacy-policy">privacy policy</Link> where you can edit your choices afterwards.
                    { showMoreOptions ?
                        <div>
                            <FormikProvider value={formik}>
                                <Form onSubmit={() => formik.submitForm()}>
                                    {
                                        AllowedConsents.map((c: Consent) => 
                                            <FormGroup key={c}>
                                                <FormControlLabel
                                                    control={<Checkbox
                                                        { ... c === Consent.Basics ? [] : formik.getFieldProps(c) }
                                                        name={`consent${c}`}
                                                        checked={formik.values[c]}
                                                        color="primary"
                                                    />}
                                                    label={ConsentDescription(c)}
                                                />
                                            </FormGroup>)
                                    }
                                    <Button key="accept" type="button" disabled={formik.isSubmitting} onClick={() => formik.submitForm()}>Accept</Button>
                                    <Button key="less" type="button" disabled={formik.isSubmitting} onClick={() => setShowMoreOptions(false)}>Less options</Button>
                                </Form>
                            </FormikProvider>
                        </div> :
                        <div>
                            <Button key="acceptall" onClick={() => setAllowAllConsent()}>Accept</Button>
                            <Button key="more" onClick={() => setShowMoreOptions(true)}>More options</Button>
                        </div>
                    }
                </Grid>
            </Grid>
        </Container>
    </div>;
}; 