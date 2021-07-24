import { IDonationSubscription } from "../../api/types";
import React, { useState } from "react";
import { ListItemAvatar, Avatar, ListItemText, ListItemSecondaryAction, IconButton, ListItem, Dialog, DialogTitle, DialogActions } from "@material-ui/core";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useMutation } from "@apollo/client/react/hooks/useMutation";
import { gql } from "@apollo/client";
import { Alert } from "../Alert/Alert";
import { GET_USER } from "../../providers/UserProvider";
import { Button } from "../Button/Button";

interface IRecurringDonationItemProps {
    subscription: IDonationSubscription;
}

const STOP_SUBSCRIPTION = gql`
    mutation StopSubscription($subscriptionId: ID!)
    {  
        donation {
            cancelSubscription(subscriptionId: $subscriptionId)
        }
    }
`;

const RecurringDonationSubscription = ({ subscription }: IRecurringDonationItemProps) => {
    const [ stopSubscription ] = useMutation(STOP_SUBSCRIPTION,
        {
            variables: {
                subscriptionId: subscription.id
            },
            onError: (data) => {
                console.error(data.message);
                setErrorMessage(data.message);
            },
            refetchQueries: [{
                query: GET_USER
            }]
        });
    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ hasStopPopup, setHasStopPopup ] = useState(false);

    return <>
        <Alert type="error" text={errorMessage} />
        <Dialog onClose={() => setHasStopPopup(false)} open={hasStopPopup}>
            <DialogTitle>
                Are you sure you want to stop this donation subscription?
            </DialogTitle>
            <DialogActions>
                <Button onClick={() => stopSubscription()}>Stop this donation subscription</Button>
                <Button onClick={() => setHasStopPopup(false)}>Don't stop this donation subscription</Button>
            </DialogActions>
        </Dialog>
        <ListItem>
            <ListItemAvatar>
            <Avatar>
                <FontAwesomeIcon icon="euro-sign" />
            </Avatar>
            </ListItemAvatar>
            <ListItemText primary={`Recurring donation started at ${subscription.startDate}`} />
            <ListItemSecondaryAction>
            <IconButton onClick={() => setHasStopPopup(true)}>
                <FontAwesomeIcon icon="trash" />
            </IconButton>
            </ListItemSecondaryAction>
        </ListItem>
    </>;
};

export default RecurringDonationSubscription;