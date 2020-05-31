import React, { useState } from "react";
import { Section } from "../../../components/Section/Section";
import { Alert } from "../../../components/Alert/Alert";
import { Grid, Checkbox, FormGroup, TextField, FormControlLabel, FormControl } from "@material-ui/core";
import { gql, useMutation } from "@apollo/client";
import styles from "./FinishRegistrationSocial.module.scss";
import { Link } from "react-router-dom";
import { useFormik, FormikProvider, Form } from "formik";
import * as Yup from "yup";
import { Button } from "../../../components/Button/Button";
import { Helmet } from "react-helmet";
import { useUser } from "../../../providers/UserProvider";

const FINISH_REGISTRATION = gql`
    mutation FinishRegistrationSocial($user: UpdatedUserInputGraph!) {
        user {
            updateUser(user: $user) {
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

const FinishRegistrationSocialPage = () => {
    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ infoMessage, setInfoMessage ] = useState<string | null>(null);
    const user = useUser();
    const [ finishRegistration ] = useMutation(
        FINISH_REGISTRATION,
        {
            onCompleted: (data) => {
                if (data.user.updateUser.result.succeeded) {
                    setErrorMessage(null);
                    setInfoMessage("You have finish your user registration. You can now start using the CollAction website.");
                } else {
                    let error = data.user.updateUser.result.errors.map((e: any) => e.description).join(", ");
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
            isSubscribedNewsletter: false,
            privacyPolicy: false
        },
        validationSchema: Yup.object({
            firstName: Yup.string(),
            lastName: Yup.string(),
            isSubscribedNewsletter: Yup.bool(),
            privacyPolicy: Yup.bool().oneOf([true], "You must agree to our privacy policy")
        }),
        onSubmit: (values) => {
            finishRegistration({
                variables: {
                    user: {
                        id: user?.id,
                        email: user?.email,
                        firstName: values.firstName,
                        lastName: values.lastName,
                        isSubscribedNewsletter: values.isSubscribedNewsletter,
                        representsNumberParticipants: 1,
                        isAdmin: user?.isAdmin
                    },
                }
            });
        }
    });

    return <>
        <Helmet>
            <title>Finish Registration Social Login</title>
            <meta name="description" content="Finish Registration" />
        </Helmet>
        <Section className={styles.intro}>
            <h1 className={styles.title}>Finish User Registration Social Login</h1>
        </Section>
        <Section color="grey">
            <Grid container justify="center">
                <Grid item sm={6}>
                    <Alert type="error" text={errorMessage} />
                    <Alert type="info" text={infoMessage} />
                    { !user ? <Alert type="warning" text="Please wait a few seconds while we finish logging you in" /> : null }
                    <FormikProvider value={formik}>
                        <Form onSubmit={formik.handleSubmit}>
                            <FormGroup>
                                <TextField name="firstName" label="First Name" type="text" { ... formik.getFieldProps('firstName') } />
                                <TextField name="lastName" label="Last Name" type="text" { ...formik.getFieldProps('lastName') } />
                                <FormControlLabel
                                    control={<Checkbox name="isSubscribedNewsletter" { ...formik.getFieldProps('isSubscribedNewsletter')} />}
                                    label={<>I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! <span role="img" aria-label="smiley">🙂</span></>} />
                                <FormControl>
                                    <FormControlLabel
                                        control={<Checkbox name="privacyPolicy" { ...formik.getFieldProps('privacyPolicy') } />}
                                        label={<>I've read and agreed to the <Link to="/privacy-policy">privacy policy</Link></>}
                                        />
                                    { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.privacyPolicy} /> : null }
                                </FormControl>
                                <Button type="submit" disabled={formik.isSubmitting || !user}>Register</Button>
                            </FormGroup>
                        </Form>
                    </FormikProvider>
                </Grid>
            </Grid>
        </Section>
    </>;
};

export default FinishRegistrationSocialPage;