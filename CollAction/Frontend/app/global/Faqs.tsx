import * as React from "react";
import Collapsible from "react-collapsible";

import renderComponentIf from "./renderComponentIf";

interface IFaqProps {
    title: any;
    content: any;
}

export default class Faq extends React.Component<IFaqProps> {
    constructor(props: IFaqProps) {
        super(props);
    }

    renderTitle() {
        return (
            <h3>
                <a href="#" className="title">{this.props.title}</a>
                <span className="toggle">
                    <i className="fa fa-plus"></i>
                    <i className="fa fa-minus"></i>
                </span>
            </h3>
        );
    }

    render() {
        return (
            <Collapsible trigger={this.renderTitle()} transitionTime={300} lazyRender={true}>
                {this.props.content}
            </Collapsible>
        );
    }
}

renderComponentIf(
    <Faq
        title="Who can start a project?"
        content={
            <span>
                <p>
                    Everyone that adheres to the start project criteria (see below) can start a
                    project: whether they are individuals, groups of friends or like minded people, or organizations.
                </p>
            </span>
        } />,
    document.getElementById("faq-who-can-start")
);

renderComponentIf(
    <Faq
        title="What would be a reasonable target number of participants?"
        content={
            <span>
                <p>
                    Setting the right target for your project is not easy. It depends on a number of different factors,
                    for instance: what type of project is it? What is the action that is proposed (is it hard or difficult)?
                    How much time do you have to promote the project? Do you have an existing network that you can mobilize?
                </p>
                <p>
                    In general, we say: it's better to set a more conservative target and far exceed it, than setting a
                    very ambitious target and not reach it by a few percent. If you want to know what a good target would
                    be for your project, a good start would be to think about your communication plan: what are you planning
                    to do to reach people? How many people can you reach through each channel that you are planning to use
                    (e.g. through your personal network, social media groups, partnering with other organizations/media)?
                    Also, please feel free to browse the projects that have already on the Find Project page, or reach out
                    to <a href="mailto:collactionteam@gmail.com">collactionteam@gmail.com</a> - we're happy to help you
                    think about this!
                </p>
            </span>
        } />,
    document.getElementById("faq-reasonable-target")
);

renderComponentIf(
    <Faq
        title="What are the criteria your project needs to meet?"
        content={
            <span>
                <p>
                    A project can only be listed when it meets the following criteria:
                </p>
                <ul>
                    <li>
                        The goal of the project is to make a positive societal or ecological contribution to your neighbourhood, country or the world.
                    </li>
                    <li>
                        The project is not geared towards personal gain.
                    </li>
                    <li>
                        The project does not include activities that are focussed on conversion or activism (religious or political).
                    </li>
                    <li>
                        The project does not include activities that are illegal or do not abide by the official legislation of the Netherlands or the country of implementation.
                    </li>
                    <li>
                        The online project registration form is completed fully and truthfully and has a clear and easily readable project description and goal. The CollAction team can ask for clarification and/or edit your text if necessary.
                    </li>
                    <li>
                        The ProjectStarter has thought through how people can be moved from commitment to action. We can help you with this!
                    </li>
                    <li>
                        The project is ambitious but realistic – the CollAction evaluation commission judges if this is the case. The ProjectStarter can activate his/her own network, and/or has a good plan to achieve the target.
                    </li>
                    <li>
                        The ProjectStarter commits to measuring the impact of the action (e.g. the number of people that have acted as a result of the project) and to sharing this information with CollAction.
                    </li>
                </ul>
            </span>
        } />,
    document.getElementById("faq-criteria")
);

renderComponentIf(
    <Faq
        title="Why should I donate?"
        content={
            <span>
                <p>
                    Our goal is to move millions of people to act for good by launching the crowdacting movement. Whereas back in the day, people turned to politicians and policy makers to fix the world’s social and environmental problems (with different levels of success :) ), we think it’s time for a new approach. With crowdacting, we take matters into our own collective hands.
                </p>
                <p>
                    But in order to reach our ambitious goals, we need you support. CollAction is a non profit organization. We keep costs super low with the support of our amazing team of volunteers and great pro bono supporters. However, there are still certain costs that need to be covered (you can find an overview of our financials <a href="https://drive.google.com/file/d/1syiUgEq-Or-GOfvBYY8k3fgTu5UZvLey/view" target="_blank">here</a>). We don’t like to be dependent on subsidies and we believe it is vital to remain independent from commercial interests. Hence, we ask for contributions from the crowd to survive, scale our impact, and remain independent.
                </p>
            </span>
        } />,
    document.getElementById("faq-why-donate")
);

renderComponentIf(
    <Faq
        title="What will my donation be spent on?"
        content={
            <span>
                <p>
                    You can find an overview of our financials <a href="https://drive.google.com/file/d/1syiUgEq-Or-GOfvBYY8k3fgTu5UZvLey/view" target="_blank">here</a> (apologies, it’s in Dutch, since that’s where our headquarter is based). In short, the main part of our budget goes to website and organizational costs (e.g. hosting, membership at a co-working space, banking costs). There are also costs related to events, but we generally break even on these events, e.g. by selling tickets, so they pay for themselves. As a team of volunteers, we manage to do a lot with just a little - your donation will go a long way. Our ambitious goal is to start the crowdacting movement and inspire millions of people to act for good by the end of 2020. All money is spent towards that goal.
                </p>
            </span>
        } />,
    document.getElementById("faq-whats-it-spent-on")
);

renderComponentIf(
    <Faq
        title="Who started CollAction?"
        content={
            <span>
                <p>
                    CollAction is started by a <a href="/About">team</a> of optimistic and pragmatic people that believe we can make this world a better place through crowdacting. The concept of CollAction/crowdacting was born in The Netherlands, but we now have an international team of around 20 volunteers.
                </p>
            </span>
        } />,
    document.getElementById("faq-who-started-it")
);

renderComponentIf(
    <Faq
        title="Is my monthly contribution tax deductible?"
        content={
            <span>
                <p>
                    Yes! Stichting CollAction has <a href="https://www.belastingdienst.nl/wps/wcm/connect/bldcontenten/belastingdienst/business/other_subjects/public_benefit_organisations" target="_blank">Public Benefit Organisation</a> (or ANBI) status in the Netherlands. This means that (part of) your <a href="https://www.belastingdienst.nl/wps/wcm/connect/bldcontenten/belastingdienst/business/other_subjects/public_benefit_organisations/tax_advantages_pbo" target="_blank">donation is tax deductible</a> if you pay taxes in the Netherlands. Different rules apply to “regular gifts” and “repeating gifts”:
                    <ul>
                        <li>
                            Regular or one-off gifts (“Gewone giften”):<br/>
                            Regular gifts (this can be either one-off gifts or recurring gifts that do not meet the “repeating gifts” criteria), can be deducted from your taxes in the Netherlands, but there is a threshold and a maximum: your donations are deductible if the total amount is at least 1% of your income, with a minimum of 60 euro. The maximum amount is 10% of your income. If you have a partner then both incomes are taken together. The difference between your donation and the threshold can be deducted from your taxes.
                        </li>
                        <li>
                            Repeating gifts (“Periodieke giften”):<br/>
                            The great thing about “repeating” gifts is that there is no threshold or maximum amount: you can deduct the full amount of the donation. However, you have to confirm in writing that you commit to giving a certain amount to CollAction over the next 5 years.
                        </li>
                    </ul>
                </p>
                <p>
                    More information on deducting your gifts can be found <a href="https://www.belastingdienst.nl/wps/wcm/connect/bldcontentnl/belastingdienst/prive/inkomstenbelasting/aftrekposten/persoonsgebonden-aftrek/giften_aan_goede_doelen/" target="_blank">here</a> (in Dutch).
                </p>
            </span>
        } />,
    document.getElementById("faq-is-it-deductable")
);



