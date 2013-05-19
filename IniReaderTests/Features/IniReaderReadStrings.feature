Feature: IniReaderReadStrings
	Given ini files as strings
    I want to test various aspects of section handling
    And apects of properties and variables handling

Scenario: Read an unnamed section
    Given file with lines
    """
    [section1]
    """
    When I get unnamed section section1
	Then The section id read is section1

Scenario: Read a named section
    Given file with lines
    """
    [section1 "name1"]
    """
    When I get named section section1 with name=name1
    Then The section id read is section1
    And The section name is name1

Scenario: File with empty section id throws exception
Given file with lines
    """
    []
    """
    When I load string
    Then The error type should be System.InvalidOperationException

Scenario: File without section with attributes throws exception
Given file with lines
    """
    host = google.com.br
    port = 81
    """
    When I load string
    Then The error type should be System.InvalidOperationException

Scenario: Test unnamed section with attributes can be handled
Given file with lines
    """
    [section]
    host = google.com.br
    port = 81
    """
    When I get unnamed section section
    Then The section id read is section
    And the host attribute is google.com.br
    And the port attribute is 81

Scenario: Test unnamed section with attribute without value can be handled
Given file with lines
    """
    [section]
    host = google.com.br
    port = 81
    useAlternatePort
    """
    When I get unnamed section section
    Then The section id read is section
    And the host attribute is google.com.br
    And the port attribute is 81
    And the useAlternatePort attribute is true

Scenario: Duplicate attribute not ignoring duplicates throws exception
Given file with lines
    """
    [section]
    host = google.com.br
    port = 81
    port = 82
    useAlternatePort
    """
    When I load string
    Then The error type should be System.Data.DuplicateNameException

Scenario: Test unnamed section with starting tabs can be handled
Given file with lines
    """
                   [section]
    host = google.com.br
    port = 81
    useAlternatePort
    """
When I get unnamed section section
    Then The section id read is section
    And the host attribute is google.com.br
    And the port attribute is 81
    And the useAlternatePort attribute is true

Scenario: Test unnamed section with tabs in attributes can be handled
Given file with lines
    """
    [section]
            host = google.com.br
                    port = 81
          useAlternatePort
    """
When I get unnamed section section
    Then The section id read is section
    And the host attribute is google.com.br
    And the port attribute is 81
    And the useAlternatePort attribute is true

Scenario: Test multiple unnamed sections in file can be handled
Given file with lines
    """
    [server]
    host = google.com
    port = 81
    useAlternatePort
    [webservice]
    host=google.com.br
    port=91
    password=123123
    """
    When I load string
    Then the attribute host from section server is google.com
    Then the attribute port from section server is 81
    Then the attribute host from section webservice is google.com.br
    Then the attribute port from section webservice is 91
    Then the attribute password from section webservice is 123123
    

Scenario: Test unnamed section with reading inexistent attribute should return null
Given file with lines
    """
    [server]
    host = google.com.br
    port = 81
    useAlternatePort
    """
When I load string
    Then the attribute host from section server is google.com.br
    And the attribute port from section server is 81
    And the attribute inexistentAttribute from section server contains null value