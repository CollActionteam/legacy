import React from 'react';
import { Grid } from '@material-ui/core';

import styles from './About.module.scss';
import { Section } from '../../components/Section/Section';
import { Faq } from '../../components/Faq/Faq';
import { useTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';
import LazyImage from '../../components/LazyImage/LazyImage';

const AboutPage = () => {
  const { t } = useTranslation();
  const team: any[] = t('about.team.team', { returnObjects: true });
  
  const videos = {
    mainvideo: 'https://www.youtube-nocookie.com/embed/xnIJo91Gero?theme=dark&amp;rel=0&amp;wmode=transparent'
  };

  const generateMemberPhoto = (member: any) => {
    const photo = require(`../../assets/${member.photo}`);
    return (
      <li key={member.name} className={styles.teamMember}>
        <LazyImage src={photo} alt={member.name} title={member.name} />
        <span>{member.name}</span>
      </li>
    );
  };

  return (
    <>
      <Helmet>
        <title>About CollAction</title>
        <meta name="description" content="About Collaction" />
      </Helmet>
      <Grid className={styles.video}>
        <iframe
          title="Collective actions"
          src={videos.mainvideo}
          frameBorder="0"
          allowFullScreen
        ></iframe>
      </Grid>
      <Section color="green" title={(t('about.mission.title'))} anchor="mission">
        <span className={styles.container} dangerouslySetInnerHTML={{ __html: t('about.mission.text') }}></span>
      </Section>
      <Section title={t('about.about.title')}>
        <span className={styles.container} dangerouslySetInnerHTML={{ __html: t('about.about.text') }}></span>
      </Section>
      <Section color="grey" title={t('about.team.title')} anchor="team">
        <span className={styles.container}>
            <ul className={styles.team}>
            {team.map(generateMemberPhoto)}
            </ul>
        </span>
      </Section>
      <Section title={t('about.join.title')}>
        <span className={styles.container} dangerouslySetInnerHTML={{ __html: t('about.join.text') }}></span>
      </Section>
      <Section color="grey" title={t('about.partners.title')} anchor="partners">
        <span className={styles.container} dangerouslySetInnerHTML={{ __html: t('about.partners.text') }}></span>
      </Section>
      <Section title="Frequently Asked Questions" anchor="faq">
        <span className={styles.container}>
            <Faq title="What is CollAction?" collapsed={false} faqId="what_is_collaction">
                <p>
                    CollAction is a not-for-profit organization based in the Netherlands that
                    helps people to solve Collective Action Problems through crowdacting.
                </p>
            </Faq>

            <Faq title="Huh? What is crowdacting?" collapsed={false} faqId="what_is_crowdacting">
                <p>
                    The term Crowdacting, as it is used here, is a term that has been introduced by CollAction.
                    Basically, it means: ‘taking action collectively and conditionally with the purpose of achieving
                    positive impact on the world and its inhabitants’. Simply put, with crowdacting we ask:
                    ‘Would you take action if you knew that a hundred/a thousand/a million/a billion others would do so too?’
                    An example of a crowdacting project: ‘If we can find 10,000 people that commit to switch to renewable
                    energy if 9,999 others would do the same, we will all switch together’. As a result, we form a
                    critical mass of people that has a huge impact on making this world a better place. The concept is very
                    similar to crowdfunding, but people commit to actions, instead of money.
                </p>
                <p>
                    In practice, a crowdacting project follows three steps:
                </p>
                <ol>
                    <li>The ProjectStarter launches a project on collaction.org. He/she proposes a certain action
                        (for example switching to renewable energy), a target (minimum number of participants), and a deadline.</li>
                    <li>ProjectSupporters commit to taking action, if the target is met within the deadline.</li>
                    <li>If and when the target is met, everybody acts collectively.</li>
                </ol>
                <p>
                    More information can be found on <a href="http://www.crowdacting.org/" target="_blank" rel="noopener noreferrer">www.crowdacting.org</a>.
                </p>
            </Faq>

            <Faq title="Crowdacting – is that a new thing?" collapsed={false} faqId="crowdacting_new">
                <p>
                    Although the term crowdacting in its current meaning is new, the underlying concept is not.
                    There are plenty of other examples of collective actions that contain elements of crowdacting.
                    Think for instance of boycotts, demonstrations, collective bargaining initiatives, or petition websites.
                    The difference is the fact that crowdacting combines three elements: <br />
                    <ol>
                        <li>It’s (explicitly) conditional (it only happens if a set target is met);</li>
                        <li>it’s for social and/or ecological good; and</li>
                        <li>the action goes beyond signing a petition. More information on how crowdacting resembles and differs from other initiatives can be found on <a href="http://www.crowdacting.org/" target="_blank" rel="noopener noreferrer">www.crowdacting.org</a>.</li>
                    </ol>
                </p>
            </Faq>

            <Faq title="What are collective action problems?" collapsed={false} faqId="collective_action_problems">
                <p>
                    Collective action problems are situations where it is in the interest of the group to work together, but
                    in which coordination is a challenge. Because of this, every individual has an incentive to act in their
                    own interest. This <a href="https://www.youtube.com/watch?v=p3KlgxYhDbk" target="_blank" rel="noopener noreferrer">video</a> explains
                    it clearly.
                </p>
                <p>
                    A well-known example of a collective action problem is the tragedy of the commons.
                    The commons is a communal meadow where cows of different farmers can graze. Every farmer wants to earn as much
                    as possible against the lowest possible costs and therefore has the individual incentive to let as many of
                    his cows graze on the communal grass as possible. However, other farmers will do the same and soon the meadow will
                    be destroyed due to overgrazing. There is little point in doing something about this for each individual farmer
                    – knowing (or fearing) that other farmers will try and use the largest possible share of the communal grass and
                    the meadow will still be overgrazed. The farmer is better off adding some more cows, so he will, at least, benefit
                    from it in the short term. However, in the long term, all the farmers and as a consequence the community will lose out,
                    because the piece of land can be written off due to overgrazing.
                </p>
                <p>
                    Examples of (serious and less serious) collective action problems are: global warming, traffic jams, overpopulation,
                    dismal working conditions, keeping a student house clean, corruption, overfishing, etc.
                </p>
                <p>
                    Traditionally, there are two solutions to collective action problems:
                </p>
                <ol>
                    <li>Privatisation: the commons can be divided between the farmers. Now the farmers benefit from taking good care of
                        their own piece of land, so that it is of use to them in the long term.</li>
                    <li>Regulation: Agreements can be made and enforced about the usage of the commons. It is, for example, possible to
                        decide on a maximum number of cows per farmer.</li>
                </ol>
                <p>
                    However, these solutions are not applicable to all collective action problems. It is not so easy (or desirable) to
                    privatise certain goods (how do you privatise the ozone layer? Or migrating fish populations?). Coming up with
                    regulations and enforce these regulations to solve (cross border) problems has proven difficult - just think about
                    the cumbersome process to do something about climate change on an international level.
                </p>
                <p>
                    So, we need a new solution to old problems: crowdacting.
                </p>
            </Faq>

            <Faq title="Where can I learn more about crowdacting?" collapsed={false} faqId="crowdacting_learn_more">
                <a href="http://www.crowdacting.org/" target="_blank" rel="noopener noreferrer">www.crowdacting.org</a>.
            </Faq>

            <Faq title="Can I start a project myself?" collapsed={false} faqId="start_project_myself">
                <p>
                    Yes! You can create a project <a href="/Projects/StartInfo" target="_blank" rel="noopener noreferrer">here</a>. Make sure to check if your idea
                    meets the CollAction criteria below.
                </p>
            </Faq>

            <Faq title="How long does a project run for?" collapsed={false} faqId="duration_project">
                <p>
                    Some actions are a one-off – think of the switch to a fair bank or green energy. Other projects can have a longer
                    duration, such as eating less meat, periodically visiting lonely elderly people or helping refugees with language
                    lessons and their integration. In other words, the duration of an action differs from one project to the other.
                </p>
            </Faq>

            <Faq title="What happens when a target has not been reached?" collapsed={false} faqId="target_not_reached">
                <p>
                    If the target is not met by the deadline, no one needs to act. Crowdacting means: No cure, no action. However,
                    if you are really inspired to take action anyway, we will of course applaud this.
                </p>
            </Faq>

            <Faq title="How do I know that other people will really take action?" collapsed={false} faqId="people_really_action">
                <p>
                    You can never be 100% sure. In the first place, crowdacting is about trust. Trusting that people want to work
                    together to create a better world. Trusting that people are attached to their personal ideals. And trusting that
                    people will keep their promises. We hope to build a community of people that take their commitments seriously and
                    try to create a culture in which agreement=agreement.
                </p>
                <p>
                    That being said, we will incorporate certain processes and tools to ensure that as many project supporters
                    proceed to taking action. What these processes are depends on the type of project. For some projects
                    (such as switching projects, for example to a fair bank or green energy) the action can already be started
                    conditionally at the time of commitment. When the target is reached, supporters of the project will automatically
                    be transferred to the fair bank or renewable energy provider. When it comes to local projects, social control will
                    play a bigger role. Other projects might be purely based on trust. We will be testing different tools and approaches
                    in the upcoming projects. We ask ProjectStarters to seriously consider this question  – the team of CollAction can
                    help you think this through!
                </p>
            </Faq>

            <Faq title="When will the projects end exactly?" collapsed={false} faqId="when_project_end">
                <p>
                    The project ends at the end (23:59:59) of the day that is listed as "Sign up closes" in the UTC (London) timezone.
                </p>
            </Faq>

            <Faq title="Where can I learn more about the CollAction organization?" collapsed={false} faqId="collaction_organisation">
                <p>
                    You can read more about our mission, strategy, and financials in <a href="https://drive.google.com/open?id=1syiUgEq-Or-GOfvBYY8k3fgTu5UZvLey">this document</a>.
                    The document is in Dutch for now, since we are headquartered in the Netherlands and need to submit it to local
                    authorities - but we're working on a translation - apologies!
                    But if you paste it in Google Translate, you should be able to get the gist ☺.
                </p>
            </Faq>

            <Faq title="Who can start a project?" collapsed={false} faqId="who_can_start">
                <p>
                    Everyone that adheres to the start project criteria (see below) can start a
                    project: whether they are individuals, groups of friends or like minded people, or organizations.
                </p>
            </Faq>

            <Faq title="What would be a reasonable target number of participants?" collapsed={false} faqId="number_participants">
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
                    to <a href="mailto:hello@collaction.org">hello@collaction.org</a> - we're happy to help you
                    think about this!
                </p>
            </Faq>

            <Faq title="What are the criteria your project needs to meet?" collapsed={false} faqId="project_criteria">
                <p>
                    A project can only be listed when it meets the following criteria:
                </p>
                <ul>
                    <li>
                        The goal of the project is to make a positive societal or ecological contribution to your neighbourhood,
                        country or the world.
                    </li>
                    <li>
                        The project is not geared towards personal gain.
                    </li>
                    <li>
                        The project does not include activities that are focussed on conversion or activism (religious or political).
                    </li>
                    <li>
                        The project does not include activities that are illegal or do not abide by the official legislation of the Netherlands or
                        the country of implementation.
                    </li>
                    <li>
                        The online project registration form is completed fully and truthfully and has a clear and easily readable project
                        description and goal. The CollAction team can ask for clarification and/or edit your text if necessary.
                    </li>
                    <li>
                        The ProjectStarter has thought through how people can be moved from commitment to action. We can help you with this!
                    </li>
                    <li>
                        The project is ambitious but realistic - The CollAction evaluation commission judges if this is the case.
                        The ProjectStarter can activate his/her own network, and/or has a good plan to achieve the target.
                    </li>
                    <li>
                        The ProjectStarter commits to measuring the impact of the action (e.g. the number of people that have acted as
                        a result of the project) and to sharing this information with CollAction.
                    </li>
                </ul>
            </Faq>

            <Faq title="Where can I find more tips and tricks on how to start, run and finish a project?" collapsed={false} faqId="project_tips_tricks">
                <p>
                    Check out our <a href="https://docs.google.com/document/d/1JK058S_tZXntn3GzFYgiH3LWV5e9qQ0vXmEyV-89Tmw" target="_blank" rel="noopener noreferrer">Project Starter Handbook</a>
                </p>
            </Faq>
        </span>
      </Section>
    </>
  );
};

export default AboutPage;
