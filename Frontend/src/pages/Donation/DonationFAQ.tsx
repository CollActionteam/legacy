import React from "react";
import styles from "./DonationFAQ.module.scss";
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import {ExpansionPanel, ExpansionPanelDetails, ExpansionPanelSummary} from "@material-ui/core";


const DonationFAQ = () => {
    return <div className={styles.list}>
        <h2>Frequently Asked Questions</h2>

        <br/>

        <div>
            <ExpansionPanel>
                <ExpansionPanelSummary
                    expandIcon={<ExpandMoreIcon/>}
                    aria-controls="panel1a-content"
                    id="panel1a-header"
                >
                    <h3>Why should I donate?</h3>
                </ExpansionPanelSummary>
                <ExpansionPanelDetails>
                    <ul>
                        <li>Our goal is to move millions of people to act for good by launching the crowdacting
                            movement. Whereas back in the day, people turned to politicians and policy makers to fix the
                            world's social and environmental problems (with different levels of success :) ), we think
                            it's time for a new approach. With crowdacting, we take matters into our own collective
                            hands.
                        </li>
                        <li>But in order to reach our ambitious goals, we need you support. CollAction is a non profit
                            organization. We keep costs super low with the support of our amazing team of volunteers and
                            great pro bono supporters. However, there are still certain costs that need to be covered
                            (you can find an overview of our financials here). We don't like to be dependent on
                            subsidies and we believe it is vital to remain independent from commercial interests. Hence,
                            we ask for contributions from the crowd to survive, scale our impact, and remain
                            independent.
                        </li>
                    </ul>
                </ExpansionPanelDetails>
            </ExpansionPanel>
            <ExpansionPanel>
                <ExpansionPanelSummary
                    expandIcon={<ExpandMoreIcon/>}
                    aria-controls="panel2a-content"
                    id="panel2a-header"
                >
                    <h3>What will my donation be spent on?</h3>
                </ExpansionPanelSummary>
                <ExpansionPanelDetails>
                    <p>
                        You can find an overview of our financials <a href="https://drive.google.com/file/d/1syiUgEq-Or-GOfvBYY8k3fgTu5UZvLey/view" target="_blank">here</a> (apologies, it's in Dutch, since that's where
                        our headquarter is based). In short, the main part of our budget goes to website and
                        organizational costs (e.g. hosting, membership at a co-working space, banking costs). There are
                        also costs related to events, but we generally break even on these events, e.g. by selling
                        tickets, so they pay for themselves. As a team of volunteers, we manage to do a lot with just a
                        little - your donation will go a long way. Our ambitious goal is to start the crowdacting
                        movement and inspire millions of people to act for good by the end of 2020. All money is spent
                        towards that goal.
                    </p>
                </ExpansionPanelDetails>
            </ExpansionPanel>
            <ExpansionPanel>
                <ExpansionPanelSummary
                    expandIcon={<ExpandMoreIcon/>}
                    aria-controls="panel3a-content"
                    id="panel3a-header"
                >
                    <h3>Who started CollAction?</h3>
                </ExpansionPanelSummary>
                <ExpansionPanelDetails>
                    <p>
                        CollAction is started by a <a href="/about#team">team</a> of optimistic and pragmatic people that believe we can make this
                        world a better place through crowdacting. The concept of CollAction/crowdacting was born in The
                        Netherlands, but we now have an international team of around 20 volunteers.
                    </p>
                </ExpansionPanelDetails>
            </ExpansionPanel>
            <ExpansionPanel>
                <ExpansionPanelSummary
                    expandIcon={<ExpandMoreIcon/>}
                    aria-controls="panel3a-content"
                    id="panel3a-header"
                >
                    <h3>How do I cancel my monthly donations?</h3>
                </ExpansionPanelSummary>
                <ExpansionPanelDetails>
                    <p>
                        You can cancel your recurring donations on your account page or by e-mailing
                        {" "}<a href="mailto:collactionteam@gmail.com">collactionteam@gmail.com</a>. Cancellations sent through e-mail will be processed within five days,
                        cancellations through your account page will be processed immediately.
                    </p>
                </ExpansionPanelDetails>
            </ExpansionPanel>
        </div>
    </div>;

};


export default DonationFAQ;