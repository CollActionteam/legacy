import { IUser, ICrowdaction } from "../../api/types";
import React, { useState } from "react";
import CrowdactionCard from "../CrowdactionCard/CrowdactionCard";
import { CardActions, CardContent, DialogContent, Dialog, DialogActions, FormGroup, TextField, Card } from "@material-ui/core";
import { Form, useFormik, FormikContext } from "formik";
import * as Yup from "yup";
import { Alert } from "../Alert/Alert";
import { useMutation, gql } from "@apollo/client";
import { Button } from "../Button/Button";
import Formatter from "../../formatter";

interface ICrowdactionParticipatingProps {
    user: IUser;
    crowdaction: ICrowdaction;
};

const SEND_CROWDACTION_EMAIL = gql`
    mutation SendCrowdactionEmail($crowdactionId: ID!, $subject: String!, $message: String!) {
        crowdaction {
            sendCrowdactionEmail(crowdactionId: $crowdactionId, subject: $subject, message: $message) {
                succeeded
                errors {
                    errorMessage
                }
                crowdaction {
                    id
                    canSendCrowdactionEmail
                    numberCrowdactionEmailsSent
                }
            }
        }
    }
`;

export default ({ user, crowdaction }: ICrowdactionParticipatingProps) => {
    const [ showSendCrowdaction, setShowSendCrowdaction ] = useState(false);
    const [ error, setError ] = useState<string | null>(null);
    const [ info, setInfo ] = useState<string | null>(null);
    const [ sendEmail ] = useMutation(
        SEND_CROWDACTION_EMAIL,
        {
            onCompleted: (data) => {
                if (data.crowdaction.sendCrowdactionEmail.succeeded) {
                    setInfo("Crowdaction E-Mail was sent");
                    setShowSendCrowdaction(false);
                } else {
                    const err = data.crowdaction.sendCrowdactionEmail.errors.map((e: { errorMessage: string }) => e.errorMessage).join(", ");
                    setError(err);
                    console.error(err);
                }
            },
            onError: (data) => {
                setError(data.message);
                console.error(data.message);
                setShowSendCrowdaction(false);
            }
        }
    )
    const formik = useFormik({
        initialValues: {
            subject: "",
            message: ""
        },
        validationSchema: Yup.object({
            subject: Yup.string().required("E-Mail must have a subject"),
            message: Yup.string().required("E-Mail must have a message")
        }),
        onSubmit: (values) => {
            sendEmail(
                {
                    variables: {
                        crowdactionId: crowdaction.id,
                        subject: values.subject,
                        message: values.message
                    }
                }
            );
        }
    });
    const crowdactionEmailsLeft = 4 - crowdaction.numberCrowdactionEmailsSent;
    const canSendUpTil = new Date(new Date(crowdaction.end).getTime() + 180 * 24 * 60 * 60 * 1000); // end date + 180 days
    return <Card>
        <Alert type="error" text={error} />
        <Alert type="info" text={info} />
        <FormikContext.Provider value={formik}>
            <Form onSubmit={formik.handleSubmit}>
                <Dialog fullScreen open={showSendCrowdaction} onClose={() => setShowSendCrowdaction(false)}>
                    <DialogContent>
                        <h3>Send crowdaction e-mail for '{crowdaction.name}' crowdaction</h3>
                        <p>Hi {user.fullName ?? "crowdaction starter"}!</p>
                        <p>
                            You can send 4 e-mails in total (during and up to 180 days after the crowdaction ends). 
                            You have already sent { crowdaction.numberCrowdactionEmailsSent } e-mails, and can still send { crowdactionEmailsLeft } emails up until { Formatter.date(canSendUpTil) } { Formatter.time(canSendUpTil) } ({ Formatter.timezone()} timezone). 
                            To personalize the message, you can add {'{firstname}'} and {'{lastname}'} to your message (including the brackets) - these will be substituted with the user's first and last name. 
                        </p>
                        <FormGroup>
                            <TextField fullWidth error={formik.submitCount > 0 && formik.errors.subject !== undefined} name="subject" label="Subject" {...formik.getFieldProps('subject')} />
                            { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.subject} /> : null }
                        </FormGroup>
                        <FormGroup>
                            <TextField fullWidth error={formik.submitCount > 0 && formik.errors.message !== undefined} multiline={true} name="message" rows={40} label="E-Mail Message" {...formik.getFieldProps('message')} />
                            { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.message} /> : null }
                        </FormGroup>
                    </DialogContent>
                    <DialogActions>
                        <Button type="submit" onClick={() => formik.submitForm()}>Send E-Mail</Button>
                        <Button type="button" onClick={() => setShowSendCrowdaction(false)}>Close</Button>
                    </DialogActions>
                </Dialog>
            </Form>
        </FormikContext.Provider>
        <CardContent>
            <CrowdactionCard crowdaction={crowdaction} />
        </CardContent>
        <CardActions>
            { crowdaction.canSendCrowdactionEmail ? <Button onClick={() => setShowSendCrowdaction(true)}>Send Crowdaction E-Mail</Button> : <Alert type="warning" text="You can't send crowdaction e-mails for this crowdaction currently. Crowdaction e-mails can be send from the moment the crowdaction has started up to 180 days after the crowdaction has ended. You can send 4 crowdaction e-mails in total. Contact hello@collaction.org if you think this is in error or if you want to raise the amount of e-mails you can send." /> }
        </CardActions>
    </Card>;
}