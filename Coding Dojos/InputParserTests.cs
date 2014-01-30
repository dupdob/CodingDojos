//  --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputParserTests.cs" company="Cyrille DUPUYDAUBY">
//   Copyright 2013 Cyrille DUPUYDAUBY
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace Coding_Dojos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NFluent;

    using NUnit.Framework;

    [TestFixture]
    public class InputParserTests
    {
        const string DefParticipants = @"Whalter White
Jesse Pinkman
Skyler White
Gustavo Fring
Saul Goodman
Henry Schrabber
Mary Schrabber
";
        [Test]
        public void LineSplitTest()
        {
            Check.That(InputParser.Parse(DefParticipants)).HasSize(7);
        }

        [Test]
        public void LineParserTest()
        {
            var testext = "Whalter White";
            var expected = new Participant();
            expected.FirstName = "Whalter";
            expected.LastName = "White";
            Check.That(InputParser.ParseLine(testext))
                 .IsInstanceOf<Participant>()
                 .And.HasFieldsEqualToThose(expected);
        }

        [Test]
        public void BasicAllocationTest()
        {
            var testtext = @"Whalter White
Jesse Pinkman";

            var participants = InputParser.Parse(testtext);

            var map = SecretSanta.DrawAll(participants);

            Check.That(map).HasSize(2);

            foreach (var participantEntry in map)
            {
                Check.That(participantEntry.Key).IsNotEqualTo(participantEntry.Value);
            }
        }

        [Test]
        public void BasicDrawTest()
        {
            var testtext = @"Whalter White
Jesse Pinkman";

            var participants = InputParser.Parse(testtext);

            var map = SecretSanta.DrawAll(participants);

            Check.That(map).HasSize(2);

            foreach (var participantEntry in map)
            {
                Check.That(participantEntry.Key).IsNotEqualTo(participantEntry.Value);
            }
        }

        [Test]
        public void FullDrawTest()
        {
            var participants = InputParser.Parse(DefParticipants);

            var map = SecretSanta.DrawAll(participants);

            Check.That(map).HasSize(7);

            foreach (var participantEntry in map)
            {
                Check.That(participantEntry.Key).IsNotEqualTo(participantEntry.Value);
            }

            var secondRun = SecretSanta.DrawAll(participants);

            Check.That(secondRun).IsNotEqualTo(map);
        }

        [Test]
        public void NoSameFamilyTest()
        {
            const string checkList = @"Whalter White
Jesse Pinkman
Skyler White
Saul Goodman";
            var participants = InputParser.Parse(checkList);

            var map = SecretSanta.DrawNotSameFamily(participants);
        }
    }

    public class SecretSanta
    {
        private static Random seed = new Random();
        public static IDictionary<Participant, Participant> DrawAll(IList<Participant> participants)
        {
            var result = new Dictionary<Participant, Participant>(participants.Count);
            for (int i = 0; i < participants.Count; i++)
            {
                result.Add(participants[i], participants[(i+1) % participants.Count]);
            }
            return result;
        }

        public static IDictionary<Participant, Participant> DrawNotSameFamily(IList<Participant> participants)
        {
            return Recurse(participants, participants);
        }

        private static Dictionary<Participant, Participant> Recurse(IEnumerable<Participant> santas, IEnumerable<Participant> candidates)
        {
            if (santas.Count() == 0)
            {
                return new Dictionary<Participant, Participant>();
            }
            var result = new Dictionary<Participant, Participant>();
            var santa = santas.ElementAt(0);
            candidates = candidates.Where(x => x.LastName != santa.LastName).ToList();
            var len = candidates.Count();
            if (len == 0)
            {
                throw new ApplicationException("Failing");
            }
            var next = seed.Next(len);
            for (var j = 0; j < len; j++)
            {
                var candidat = candidates.ElementAt((j+next) % len);
                try
                {
                    result = Recurse(
                        santas.Where(x => x != santa).ToList(),
                        candidates.Where(x => x != candidat).ToList());
                    result[santa] = candidat;
                    return result;
                }
                catch (ApplicationException)
                {
                    // failed
                }
            }
            throw new ApplicationException("Failing");
        }
    }

    public class InputParser
    {
        public static IList<Participant> Parse(string testtext)
        {
            var pos = 0;
            var result = new List<Participant>();
            while (true)
            {
                var next = testtext.IndexOf(Environment.NewLine, pos);
                if (next == -1)
                {
                    var substring = testtext.Substring(pos);
                    if (!string.IsNullOrEmpty(substring))
                        result.Add(ParseLine(substring));
                    break;
                }
                else
                {
                    var substring = testtext.Substring(pos, next-pos);
                    if (!string.IsNullOrEmpty(substring))
                        result.Add(ParseLine(substring));
                    pos = next + Environment.NewLine.Length;
                }
            }
            return result;
        }

        public static Participant ParseLine(string testext)
        {
            var result = new Participant();
            var temp = testext.Split(' ');
            if (temp.Length != 2)
            {
                throw new ArgumentException("Not properly formatted name");
            }
            result.FirstName = temp[0];
            result.LastName = temp[1];
            return result;
        }

    }


    public class Participant
    {
        protected bool Equals(Participant other)
        {
            return string.Equals(this.FirstName, other.FirstName) && string.Equals(this.LastName, other.LastName);
        }

        /// <summary>
        /// Détermine si l'objet <see cref="T:System.Object"/> spécifié est égal à l'objet <see cref="T:System.Object"/> actuel.
        /// </summary>
        /// <returns>
        /// true si l'objet spécifié est égal à l'objet actuel ; sinon, false.
        /// </returns>
        /// <param name="obj">Objet à comparer avec l'objet actif.</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Participant)obj);
        }

        /// <summary>
        /// Sert de fonction de hachage pour un type particulier.
        /// </summary>
        /// <returns>
        /// Code de hachage du <see cref="T:System.Object"/> actuel.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.FirstName != null ? this.FirstName.GetHashCode() : 0) * 397) ^ (this.LastName != null ? this.LastName.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Participant left, Participant right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Participant left, Participant right)
        {
            return !Equals(left, right);
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FulleName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}
