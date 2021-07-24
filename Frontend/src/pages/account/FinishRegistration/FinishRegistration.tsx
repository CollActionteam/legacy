import React, { useState } from "react";
import { Section } from "../../../components/Section/Section";
import { Alert } from "../../../components/Alert/Alert";
import { Grid, Checkbox, FormGroup, TextField, FormControlLabel, FormControl } from "@material-ui/core";
import { gql, useMutation } from "@apollo/client";
import styles from "./FinishRegistration.module.scss";
import { Link, useLocation } from "react-router-dom";
import { useFormik, FormikProvider, Form } from "formik";
import * as Yup from "yup";
import { Button } from "../../../components/Button/Button";
import { Helmet } from "react-helmet";

const FINISH_REGISTRATION = gql`
    mutation FinishRegistration($user: NewUserInputGraph!, $code: String!) {
        user {
            finishRegistration(user: $user, code: $code) {
                result {
                    succeeded
                    errors {
                        code
                        description
                    }
                }
            }
        }
    }
`;

const FinishRegistrationPage = () => {
    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ infoMessage, setInfoMessage ] = useState<string | null>(null);
    const [ isSuccessfull, setIsSuccessfull ] = useState(false);
    const searchParams = new URLSearchParams(useLocation().search);
    const email = searchParams.get("email");
    const code = searchParams.get("code");
    const [ finishRegistration ] = useMutation(
        FINISH_REGISTRATION,
        {
            onCompleted: (data) => {
                if (data.user.finishRegistration.result.succeeded) {
                    setErrorMessage(null);
                    setInfoMessage("You have been registered. You can now login with your new account.");
                    formik.resetForm({});
                    setIsSuccessfull(true);
                } else {
                    let error = data.user.finishRegistration.result.errors.map((e: any) => e.description).join(", ");
                    setInfoMessage(null);
                    setErrorMessage(error);
                }
            },
            onError: (data) => {
                setErrorMessage(data.message);
                console.error(data.message);
            }
        }
    );
    const formik = useFormik({
        initialValues: {
            firstName: "",
            lastName: "",
            password: "",
            confirmPassword: "",
            isSubscribedNewsletter: false,
            privacyPolicy: false
        },
        validationSchema: Yup.object({
            firstName: Yup.string(),
            lastName: Yup.string(),
            password: Yup.string().min(6, "Password must be at least six characters long")
                                  .required("Must fill in a password at least six characters long"),
            confirmPassword: Yup.string().required("Must fill in a confirmation password")
                                         .when("password", {
                is: (val: string | any[]) => val && val.length > 0,
                then: Yup.string()
                         .oneOf([Yup.ref("password")], "Both passwords need to be the same")
                         .required("Must fill in a confirmation password")
            }),
            isSubscribedNewsletter: Yup.bool(),
            privacyPolicy: Yup.bool().oneOf([true], "You must agree to our privacy policy")
        }),
        onSubmit: (values) => {
            finishRegistration({
                variables: {
                    user: {
                        firstName: values.firstName,
                        lastName: values.lastName,
                        email: email,
                        password: values.password,
                        isSubscribedNewsletter: values.isSubscribedNewsletter
                    },
                    code: code
                }
            });
        }
    });

    return <>
        <Helmet>
            <title>Finish Registration</title>
            <meta name="description" content="Finish Registration" />
        </Helmet>
        <Section className={styles.intro}>
            <h1 className={styles.title}>Finish User Registration</h1>
        </Section>
        <Section color="grey">
            <Grid container justify="center">
                <Grid item sm={6}>
                    <Alert type="error" text={errorMessage} />
                    <Alert type="info" text={infoMessage} />
                    <FormikProvider value={formik}>
                        <Form onSubmit={formik.handleSubmit}>
                            <FormGroup>
                                <TextField label="First Name" type="text" { ... formik.getFieldProps('firstName') } />
                                <TextField label="Last Name" type="text" { ...formik.getFieldProps('lastName') } />
                                <TextField label="Password" type="password" error={formik.touched.password && formik.errors.password !== undefined} { ...formik.getFieldProps('password') } />
                                { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.password} /> : null }
                                <TextField label="Confirm Password" type="password" error={formik.touched.confirmPassword && formik.errors.confirmPassword !== undefined} { ...formik.getFieldProps('confirmPassword') } />
                                { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.confirmPassword} /> : null }
                                <FormControlLabel
                                    control={<Checkbox { ...formik.getFieldProps('isSubscribedNewsletter')} />}
                                    label={<>I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! <span role="img" aria-label="smiley">🙂</span></>} />
                                <FormControl>
                                    <FormControlLabel
                                        control={<Checkbox { ...formik.getFieldProps('privacyPolicy') } />}
                                        label={<>I've read and agreed to the <Link to="/privacy-policy">privacy policy</Link></>}
                                        />
                                    { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.privacyPolicy} /> : null }
                                </FormControl>
                                <Button type="submit" disabled={isSuccessfull}>Register</Button>
                            </FormGroup>
                        </Form>
                    </FormikProvider>
                </Grid>
            </Grid>
        </Section>
    </>;
};

export default FinishRegistrationPage;